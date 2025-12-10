using Domain.Entities.Company;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class FreelancerRepository : BaseRepository<Freelancer>, IFreelancerRepository
    {
        public FreelancerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Freelancer?> GetByAppUserIdAsync(Guid appUserId, CancellationToken token)
        {
            return await _context.Set<Freelancer>()
                                 .FirstOrDefaultAsync(c => c.AppUserId == appUserId);
        }
    }
}
