using Domain.Entities.Companies;
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

        public async Task<List<(Worker Worker, Company Company)>> GetAllByAppUserIdWithCompanyAsync(Guid appUserId, CancellationToken token)
        {
            var query = from w in _context.Workers
                        join c in _context.Companies on w.CompanyId equals c.Id
                        where w.AppUserId == appUserId
                        select new { w, c };

            var result = await query.AsNoTracking().ToListAsync(token);

            return result.Select(x => (x.w, x.c)).ToList();
        }
    }
}
