using Domain.Entities;

namespace Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default);
        void Add(User user);
        void Update(User user);
    }
}
