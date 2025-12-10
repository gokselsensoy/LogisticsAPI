using Domain.Entities.Customer;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CorporateCustomerRepository : BaseRepository<CorporateCustomer>, ICorporateCustomerRepository
    {
        public CorporateCustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<CorporateCustomer?> GetByResponsibleAppUserIdAsync(Guid appUserId, CancellationToken token)
        {
            return await _context.Set<CorporateCustomer>()
                                 .Include(c => c.Responsibles)
                                 .FirstOrDefaultAsync(c => c.Responsibles.Any(r => r.AppUserId == appUserId), token);
        }
    }
}
