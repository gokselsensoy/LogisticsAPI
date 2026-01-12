using Domain.Enums;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Orders
{
    public class Shipment : Entity, IAggregateRoot
    {
        public Guid? OrderId { get; private set; }
        public Guid? ReturnRequestId { get; private set; }

        public string? ExternalShipmentRef { get; private set; }

        public ShipmentType Type { get; private set; }

        // Bu işi kim başlattı? (Order mı? Yoksa Şoför "Gidip bakayım" mı dedi?)
        public ShipmentSource SourceType { get; private set; }

        public Address PickupAddress { get; private set; }
        public Address DeliveryAddress { get; private set; }


        public Guid AssignedTransporterId { get; private set; }

        public ShipmentStatus Status { get; private set; }

        private readonly List<ShipmentItem> _items = new();
        public IReadOnlyCollection<ShipmentItem> Items => _items.AsReadOnly();

        private Shipment() { }

        public Shipment(
            ShipmentType type,
            ShipmentSource sourceType,
            Address pickup,
            Address delivery,
            Guid? orderId = null,
            Guid? returnId = null)
        {
            Id = Guid.NewGuid();
            Type = type;
            SourceType = sourceType;
            PickupAddress = pickup;
            DeliveryAddress = delivery;
            OrderId = orderId;
            ReturnRequestId = returnId;
            Status = ShipmentStatus.Pending;
        }

        public void AddShipmentItem(Guid orderItemId, Guid? packageId, CargoSpec spec, int plannedQuantity)
        {
            // Validasyon (Opsiyonel): Aynı orderItem daha önce eklendi mi?
            if (_items.Any(x => x.OrderItemId == orderItemId))
            {
                // Aynı sipariş kalemi parça parça da eklenebilir, o yüzden hata fırlatmak yerine
                // lojistik mantığına göre karar verilmeli. Şimdilik direkt ekleyelim.
            }

            var item = new ShipmentItem(Id, orderItemId, packageId, spec, plannedQuantity);
            _items.Add(item);
        }

        // Proaktif (Şoförün başlattığı) boş shipment için
        public static Shipment CreateProactive(Address targetAddress)
        {
            return new Shipment(ShipmentType.Pickup, ShipmentSource.TransporterInitiated, targetAddress, targetAddress);
        }
    }
}