using Domain.Entities.Inventory;

namespace Domain.Entities.Company
{
    public class Supplier : Company
    {
        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        public Supplier(string name, string? cvrNumber) : base(name, cvrNumber) { }

        public void AddProduct(Product product) => _products.Add(product);
    }
}