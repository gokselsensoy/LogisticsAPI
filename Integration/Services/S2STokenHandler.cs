using Duende.IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace Integration.Services
{
    public class S2STokenHandler : DelegatingHandler
    {
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private static readonly HttpClient _tokenClient = new HttpClient();
        private const string CacheKey = "S2S_ACCESS_TOKEN";

        public S2STokenHandler(IDistributedCache cache, IConfiguration configuration) // <-- Değişti
        {
            _cache = cache;
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _cache.GetStringAsync(CacheKey, cancellationToken);

            if (string.IsNullOrEmpty(token))
            {
                var tokenEndpoint = _configuration["Auth:Authority"] + "/connect/token";

                var tokenResponse = await _tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = tokenEndpoint,
                    ClientId = "logistics_api_service",
                    ClientSecret = _configuration["IdentityApi:ClientSecret"]
                }, cancellationToken);

                if (tokenResponse.IsError)
                {
                    throw new InvalidOperationException($"S2S Token alınamadı: {tokenResponse.Error}");
                }

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 60)
                };
                await _cache.SetStringAsync(CacheKey, tokenResponse.AccessToken, cacheOptions, cancellationToken);

                token = tokenResponse.AccessToken;
            }
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
