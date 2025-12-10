using Domain.SeedWork;

namespace Domain.Entities.Inventory
{
    // =========================================================================
    // 3. PRODUCT & INVENTORY (ÜRÜN VE STOK)
    // =========================================================================

    public class Product : Entity, IAggregateRoot
    {
        public Guid SupplierId { get; private set; }
        public string Name { get; private set; }

        private readonly List<Package> _packages = new();
        public IReadOnlyCollection<Package> Packages => _packages.AsReadOnly();

        private Product() { }

        public Product(Guid supplierId, string name)
        {
            Id = Guid.NewGuid();
            SupplierId = supplierId;
            Name = name;
        }
    }
}