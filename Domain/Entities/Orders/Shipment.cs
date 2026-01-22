using Domain.Enums;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Orders
{
    public class Shipment : Entity, IAggregateRoot
    {
        public Guid? OrderId { get; private set; }
        public Guid? ReturnRequestId { get; private set; }

        // --- KİMLİK (Denormalizasyon) ---
        // Transporter, "Samsung (Supplier) deposundan alacaklarımı ver" dediğinde hızlı çalışması için.
        public Guid? SupplierId { get; private set; }
        public Guid AssignedTransporterId { get; private set; }

        public string? ExternalShipmentRef { get; private set; }
        public ShipmentType Type { get; private set; }
        public ShipmentSource SourceType { get; private set; }
        public ShipmentStatus Status { get; private set; }

        public Address PickupAddress { get; private set; }
        public Address DeliveryAddress { get; private set; }

        private readonly List<ShipmentItem> _items = new();
        public IReadOnlyCollection<ShipmentItem> Items => _items.AsReadOnly();

        private Shipment() { }

        public Shipment(
            ShipmentType type,
            ShipmentSource sourceType,
            Address pickup,
            Address delivery,
            Guid transporterId,
            Guid? supplierId = null, // Order'dan gelir
            Guid? orderId = null,
            Guid? returnId = null)
        {
            Id = Guid.NewGuid();
            Type = type;
            SourceType = sourceType;
            PickupAddress = pickup;
            DeliveryAddress = delivery;
            AssignedTransporterId = transporterId;

            SupplierId = supplierId; // Denormalized ID set edilir
            OrderId = orderId;
            ReturnRequestId = returnId;

            Status = ShipmentStatus.Pending;
        }

        public void AddShipmentItem(Guid orderItemId, Guid packageId, CargoSpec spec, int plannedQuantity)
        {
            // Mükerrer kayıt kontrolü veya lojistik validasyonları buraya eklenebilir.
            var item = new ShipmentItem(Id, orderItemId, packageId, spec, plannedQuantity);
            _items.Add(item);

            // Event fırlatabiliriz: "ShipmentItemAdded" -> OrderItem.ShippedQuantity update edilir.
        }

        // Proaktif (Şoför inisiyatifi) shipment
        public static Shipment CreateProactive(Address targetAddress, Guid transporterId)
        {
            return new Shipment(
                ShipmentType.Pickup,
                ShipmentSource.TransporterInitiated,
                targetAddress,
                targetAddress,
                transporterId);
        }
    }
}