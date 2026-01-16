using Domain.Entities.Customers;
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

        public async Task<CorporateResponsible?> GetByIdWithCustomerAsync(Guid id, CancellationToken token)
        {
            return await _context.Set<CorporateResponsible>()
                .AsNoTracking() // Sadece okuma yapıp DTO dolduracaksak performans için
                .Include(r => r.CorporateCustomer) // <--- JOIN İŞLEMİ
                .FirstOrDefaultAsync(r => r.Id == id, token);
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

        public async Task<List<CorporateResponsible>> GetByCorporateIdAsync(Guid corporateId, CancellationToken token)
        {
            return await _context.Set<CorporateResponsible>()
                .Where(r => r.CorporateCustomerId == corporateId && !r.IsDeleted)
                .ToListAsync(token);
        }

        // ID ile getirir ama AssignedAddresses (Atanmış Şubeler) ilişkisini de yükler
        public async Task<CorporateResponsible?> GetByIdWithAssignmentsAsync(Guid id, CancellationToken token)
        {
            return await _context.Set<CorporateResponsible>()
                .Include(r => r.AssignedAddresses) // Yetki kontrolü için şart
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, token);
        }
    }
}
