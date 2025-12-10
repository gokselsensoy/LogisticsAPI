using Application.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(AppUser user)
        {
            _context.AppUsers.Add(user);
        }

        public void Update(AppUser user)
        {
            _context.AppUsers.Update(user);
        }

        public async Task<AppUser?> GetByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default)
        {
            return await _context.AppUsers
                .FirstOrDefaultAsync(u => u.IdentityId == identityId, cancellationToken);
        }

        public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Set<AppUser>()
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}
