using Domain.SeedWork;

namespace Domain.Entities.Orders
{
    public class Basket : Entity, IAggregateRoot
    {
        public Guid CustomerId { get; private set; }

        private readonly List<BasketItem> _items = new();
        public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

        private Basket() { }

        public Basket(Guid customerId)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
        }

        public void AddItem(Guid packageId, Guid supplierId, int quantity)
        {
            var existingItem = _items.FirstOrDefault(x => x.PackageId == packageId);
            if (existingItem != null)
            {
                existingItem.IncreaseQuantity(quantity);
            }
            else
            {
                _items.Add(new BasketItem(Id, packageId, supplierId, quantity));
            }
        }

        public void RemoveItem(Guid packageId, int quantity = 0)
        {
            var item = _items.FirstOrDefault(x => x.PackageId == packageId);
            if (item == null) return;

            if (quantity > 0 && item.Quantity > quantity)
            {
                _items.Remove(item);
            }
            else
            {
                // Tamamen sepetten at
                _items.Remove(item);
            }
        }

        public decimal CalculateTotal(Dictionary<Guid, decimal> priceMap)
        {
            return _items.Sum(i => i.Quantity * (priceMap.ContainsKey(i.PackageId) ? priceMap[i.PackageId] : 0));
        }

        public void ClearItems() => _items.Clear();
    }
}