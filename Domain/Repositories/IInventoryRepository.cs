using Domain.Entities.Inventory;
using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        // Lokasyon koduna göre ara (Unique check için)
        Task<bool> IsLocationExistsAsync(Guid terminalId, string locationCode, CancellationToken token);

        // ID ile getir ama Stokları da dahil et (Include Stocks)
        Task<Inventory?> GetByIdWithStocksAsync(Guid id, CancellationToken token);

        Task<Inventory?> GetFirstWithStockAsync(Guid packageId, Guid ownerId, int quantity, InventoryState state, CancellationToken token);
    }
}
