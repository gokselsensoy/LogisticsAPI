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

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public async Task<User?> GetByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.IdentityId == identityId, cancellationToken);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }
    }
}
