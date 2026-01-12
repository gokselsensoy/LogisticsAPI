using Domain.Entities.Customers;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IndividualCustomer?> GetByAppUserIdAsync(Guid appUserId, CancellationToken token)
        {
            return await _context.Set<IndividualCustomer>() // Set<IndividualCustomer> diyerek sadece bireyselleri hedefliyoruz
                .FirstOrDefaultAsync(c => c.AppUserId == appUserId && !c.IsDeleted, token);
        }

        // Customer'ı getir (Bireysel veya Kurumsal fark etmez) ama Adreslerini de yükle
        public async Task<Customer?> GetByIdWithAddressesAsync(Guid id, CancellationToken token)
        {
            return await _context.Set<Customer>()
                .Include(c => c.Addresses.Where(a => !a.IsDeleted)) // Sadece silinmemiş adresler
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, token);
        }
    }
}
