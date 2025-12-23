using Domain.Entities.Inventory;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Product?> GetByIdWithPackagesAsync(Guid id, CancellationToken token)
        {
            return await _context.Set<Product>()
                .Include(p => p.Packages.Where(pkg => !pkg.IsDeleted)) // Sadece silinmemiş paketler
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, token);
        }

        public async Task<bool> IsBarcodeExistAsync(string barcode, CancellationToken token)
        {
            return await _context.Set<Package>()
               .AnyAsync(p => p.Barcode == barcode && !p.IsDeleted, token);
        }
    }
}
