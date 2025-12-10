using Domain.Entities.Customer;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class IndividualCustomerRepository : BaseRepository<IndividualCustomer>, IIndividualCustomerRepository
    {
        public IndividualCustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<IndividualCustomer?> GetByAppUserIdAsync(Guid appUserId, CancellationToken token)
        {
            return _context.Set<IndividualCustomer>()
                                 .FirstOrDefaultAsync(c => c.AppUserId == appUserId);
        }
    }
}
