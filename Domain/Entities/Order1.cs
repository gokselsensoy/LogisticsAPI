using Domain.SeedWork;

namespace Domain.Entities
{
    // =========================================================================
    // 4. ORDER & SHIPMENT (SİPARİŞ VE SEVKİYAT)
    // =========================================================================

    public class Order : Entity, IAggregateRoot
    {
        public Guid CustomerId { get; private set; }
        public CustomerType CustomerType { get; private set; }

        public Guid SupplierId { get; private set; }

        // Malın teslim edileceği nokta (Snapshot)
        public Location DeliveryLocation { get; private set; }
        public Guid? SavedAddressId { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public Order(Guid customerId, CustomerType type, Guid supplierId, Location deliveryLocation)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            CustomerType = type;
            SupplierId = supplierId;
            DeliveryLocation = deliveryLocation;
        }
    }
}