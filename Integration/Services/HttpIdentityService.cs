using Application.Abstractions.Services;
using Application.Features.Auth.DTOs;
using Application.Shared.Models;
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

        #region Get By Email Async
        public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            // IdentityAPI'de bu endpoint'in olduğunu varsayıyoruz:
            // GET /api/users/by-email/{email}
            try
            {
                var response = await _httpClient.GetFromJsonAsync<UserDto>(
                    $"/api/users/by-email/{email}",
                    cancellationToken);

                return response;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Kullanıcı yoksa null dön
                return null;
            }
        }
        #endregion

        #region Create User Async
        // Genel Kullanıcı Oluşturma (Worker, Customer, Freelancer hepsi kullanır)
        public async Task<Guid?> CreateUserAsync(string email, string password, string role, CancellationToken cancellationToken)
        {
            var request = new
            {
                Email = email,
                Password = password,
                Role = role
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/internal-register", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                // Hatayı loglamak veya fırlatmak daha iyidir
                throw new Exception($"IdentityAPI Hatası ({response.StatusCode}): {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<CreateInternalUserResponse>(cancellationToken: cancellationToken);
            return result?.UserId;
        }

        private record CreateInternalUserResponse(Guid UserId, string Email);
        #endregion

        #region Login
        public async Task<TokenResponse?> LoginAsync(string email, string password, string clientType, CancellationToken cancellationToken)
        {
            var requestData = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "username", email },
            { "password", password },
            { "client_id", clientType },
            // { "client_secret", "..." } // Eğer public client değilse secret gerekir
            { "scope", "offline_access logistics_api" }
        };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/connect/token")
            {
                Content = new FormUrlEncodedContent(requestData)
            };

            // ÖNEMLİ: Eğer HttpClient'ında otomatik token ekleyen bir yapı varsa,
            // bu istek özelinde Authorization header'ını boşaltıyoruz.
            // Çünkü Login isteği "Anonymous" (Kimliksiz) yapılır.
            requestMessage.Headers.Authorization = null;

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                // Hatanın detayını görmek için (Debug amaçlı)
                var errorContent = await response.Content.ReadAsStringAsync();
                // Console.WriteLine(errorContent); // Loglayabilirsin
                return null;
            }

            return await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
        }
        #endregion
    }
}
