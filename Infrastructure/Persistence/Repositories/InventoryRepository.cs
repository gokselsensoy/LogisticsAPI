using Domain.Entities.Inventory;
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
    }
}
