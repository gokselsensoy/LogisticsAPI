using Domain.Entities.Inventory;
using Domain.Events;

namespace Domain.Entities.Company
{
    public class Supplier : Company
    {
        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        private Supplier() : base(null!, null) { }

        public Supplier(string name, string? cvrNumber, string email) : base(name, cvrNumber)
        {
            AddDomainEvent(new SupplierRegisteredEvent(this.Id, name, email));
        }

        public void AddProduct(Product product) => _products.Add(product);
    }
}