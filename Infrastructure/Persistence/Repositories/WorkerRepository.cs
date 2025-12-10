using Domain.Entities.Departments;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class WorkerRepository : BaseRepository<Worker>, IWorkerRepository
    {
        public WorkerRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Worker?> GetByAppUserIdWithCompanyAsync(Guid appUserId, CancellationToken token)
        {
            return await _context.Set<Worker>()
                                 .FirstOrDefaultAsync(w => w.AppUserId == appUserId, token);
        }
    }
}
