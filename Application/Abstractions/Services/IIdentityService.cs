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
    }
}
