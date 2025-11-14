using Application.Abstractions.Services;
using System.Net.Http.Json;

namespace Integration.Services
{
    public class HttpIdentityService : IIdentityService
    {
        // DI, buraya S2S token'ını OTOMATİK ekleyen HttpClient'ı verecek
        private readonly HttpClient _httpClient;

        public HttpIdentityService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Guid?> CreateWorkerUserAsync(string email, string password, CancellationToken cancellationToken)
        {
            var request = new
            {
                Email = email,
                Password = password,
                Role = "Worker"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/internal-register", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                // TODO: Loglama yapılmalı
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<CreateInternalUserResponse>(cancellationToken: cancellationToken);
            return result?.UserId;
        }

        private record CreateInternalUserResponse(Guid UserId, string Email);
    }
}
