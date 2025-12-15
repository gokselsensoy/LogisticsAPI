using Domain.Entities.Customer;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CorporateResponsibleRepository : BaseRepository<CorporateResponsible>, ICorporateResponsibleRepository
    {
        public CorporateResponsibleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<CorporateResponsible?> GetByAppUserIdAsync(Guid appUserId, CancellationToken token)
        {
            return _context.Set<CorporateResponsible>()
                                 .FirstOrDefaultAsync(c => c.AppUserId == appUserId);
        }

        public async Task<List<(CorporateResponsible Responsible, CorporateCustomer Customer)>> GetResponsiblesWithCustomerAsync(Guid appUserId, CancellationToken token)
        {
            var query = from r in _context.CorporateResponsibles
                        join c in _context.CorporateCustomers on r.CorporateCustomerId equals c.Id
                        where r.AppUserId == appUserId
                        // Include YOK, Select ile projeksiyon var (Performans + Decoupling)
                        select new { r, c };

            var result = await query.AsNoTracking().ToListAsync(token);

            return result.Select(x => (x.r, x.c)).ToList();
        }
    }
}
