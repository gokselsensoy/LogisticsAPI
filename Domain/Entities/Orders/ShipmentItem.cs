using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Orders
{
    public class ShipmentItem : Entity
    {
        public Guid ShipmentId { get; private set; }

        // Hangi sipariş kalemine ait? (Geriye dönük takip için şart)
        public Guid? OrderItemId { get; private set; }
        public Guid? ReturnItemId { get; private set; }
        public Guid? PackageId { get; private set; }

        public CargoSpec Spec { get; private set; }
        public int PlannedQuantity { get; private set; } // Planlanan

        // Gerçekleşenler (Saha Operasyonu)
        public int LoadedQuantity { get; private set; }
        public int DeliveredQuantity { get; private set; }
        public int RejectedQuantity { get; private set; }
        public string? RejectionReason { get; private set; }

        private ShipmentItem() { }

        public ShipmentItem(Guid shipmentId, Guid orderItemId, Guid packageId, CargoSpec spec, int quantity)
        {
            ShipmentId = shipmentId;
            OrderItemId = orderItemId;
            PackageId = packageId;
            Spec = spec;
            PlannedQuantity = quantity;
        }

        // Constructor: İade Toplama İçin (Overload)
        public ShipmentItem(Guid shipmentId, Guid returnItemId, Guid packageId, CargoSpec spec, int quantity, bool isReturn)
        {
            ShipmentId = shipmentId;
            ReturnItemId = returnItemId; // OrderItem yerine ReturnItem bağlanır
            PackageId = packageId;
            Spec = spec;
            PlannedQuantity = quantity;
        }

        public void UpdateActuals(int loaded, int delivered, int rejected, string? reason)
        {
            LoadedQuantity = loaded;
            DeliveredQuantity = delivered;
            RejectedQuantity = rejected;
            RejectionReason = reason;
        }
    }
}