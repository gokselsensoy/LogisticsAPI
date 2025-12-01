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
    }
}
