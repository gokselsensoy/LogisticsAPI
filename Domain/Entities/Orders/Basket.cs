using Domain.SeedWork;

namespace Domain.Entities.Orders
{
    public class Basket : Entity, IAggregateRoot
    {
        public Guid CustomerId { get; private set; }
        private readonly List<BasketItem> _items = new();
        public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

        private Basket() { }
        public Basket(Guid customerId) { Id = Guid.NewGuid(); CustomerId = customerId; }

        public void AddItem(Guid packageId, Guid supplierId, int quantity)
        {
            var existing = _items.FirstOrDefault(x => x.PackageId == packageId);
            if (existing != null) existing.IncreaseQuantity(quantity);
            else _items.Add(new BasketItem(Id, packageId, supplierId, quantity));
        }

        public void RemoveItem(Guid packageId, int quantity = 0)
        {
            var item = _items.FirstOrDefault(x => x.PackageId == packageId);
            if (item == null) return;
            if (quantity > 0 && item.Quantity > quantity) _items.Remove(item);
            else _items.Remove(item);
        }

        public void ClearItems() => _items.Clear();
    }
}