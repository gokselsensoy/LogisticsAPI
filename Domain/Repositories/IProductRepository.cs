using Domain.Entities.Inventory;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByIdWithPackagesAsync(Guid id, CancellationToken token);
        Task<bool> IsBarcodeExistAsync(string barcode, CancellationToken token);
        Task<Package?> GetPackageByIdAsync(Guid packageId, CancellationToken token);
    }
}
