using Application.Features.Auth.DTOs;
using Application.Shared.Models;

namespace Application.Abstractions.Services
{
    public interface IIdentityService
    {
        Task<Guid?> CreateUserAsync(string email, string password, string role, CancellationToken cancellationToken);
        Task<TokenResponse?> LoginAsync(string email, string password, string clientType, CancellationToken cancellationToken);
        Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task AddToRoleAsync(Guid identityId, string role, CancellationToken token);

        Task<TokenResponse> CreateTokenForProfileAsync(Guid userId, Guid appUserId, Guid? companyId, string profileType, Guid? profileId, List<string> roles, string clientId, CancellationToken token);
        Task ForgotPasswordAsync(string email, CancellationToken cancellationToken);
        Task VerifyCodeAsync(string email, string code, CancellationToken cancellationToken);
        Task ResetPasswordAsync(string email, string code, string newPassword, CancellationToken cancellationToken);
    }
}
