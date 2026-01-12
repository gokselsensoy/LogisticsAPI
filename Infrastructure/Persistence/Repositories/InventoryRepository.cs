using Domain.Entities.Inventories;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsLocationExistsAsync(Guid terminalId, string locationCode, CancellationToken token)
        {
            return await _context.Set<Inventory>()
                .AnyAsync(x => x.TerminalId == terminalId && x.LocationCode == locationCode && !x.IsDeleted, token);
        }

        public async Task<Inventory?> GetByIdWithStocksAsync(Guid id, CancellationToken token)
        {
            return await _context.Set<Inventory>()
                .Include(i => i.Stocks) // Stokları da çekiyoruz
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, token);
        }

        public async Task<Inventory?> GetFirstWithStockAsync(Guid packageId, Guid ownerId, int quantity, InventoryState state, CancellationToken token)
        {
            // Stocks listesinin içinde aradığımız kriterde stok var mı?
            // Inventory -> Stocks (Include)
            return await _context.Set<Inventory>()
                .Include(i => i.Stocks)
                .Where(i => !i.IsDeleted) // Depo silinmemiş
                .Where(i => i.Stocks.Any(s =>
                    s.PackageId == packageId &&
                    s.OwnerId == ownerId &&
                    s.State == state &&
                    s.Quantity >= quantity)) // Yeterli stok var mı?
                .FirstOrDefaultAsync(token);
        }
    }
}
