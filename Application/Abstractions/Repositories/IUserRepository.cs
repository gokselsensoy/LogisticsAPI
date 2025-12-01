using Domain.Entities;

namespace Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default);
        void Add(AppUser user);
        void Update(AppUser user);
    }
}
