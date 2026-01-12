using Domain.Entities.Order;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class BasketRepository : BaseRepository<Basket>, IBasketRepository
    {
        public BasketRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Basket?> GetByCustomerIdAsync(Guid customerId, CancellationToken token)
        {
            return await _context.Set<Basket>()
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.CustomerId == customerId, token);
        }

        public async Task<Basket?> GetByIdWithItemsAsync(Guid id, CancellationToken token)
        {
            return await _context.Set<Basket>()
                .Include(b => b.Items) // Itemları çekiyoruz
                .FirstOrDefaultAsync(b => b.Id == id, token); // Soft delete yoksa IsDeleted kontrolüne gerek yok (Basket genelde Redis veya geçici tablodur)
        }
    }
}
