using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Orders
{
    public class ShipmentItem : Entity
    {
        public Guid ShipmentId { get; private set; }
        public Guid? OrderItemId { get; private set; }

        public Guid? PackageId { get; private set; }
        public CargoSpec Spec { get; private set; }

        public int PlannedQuantity { get; private set; }

        public int LoadedQuantity { get; private set; } // Araca binen
        public int DeliveredQuantity { get; private set; } // Teslim edilen
        public int RejectedQuantity { get; private set; } // İade/Kabul edilmeyen
        public string? RejectionReason { get; private set; }

        public ShipmentItem(Guid shipmentId, Guid? orderItemId, Guid? packageId, CargoSpec spec, int plannedQuantity)
        {
            ShipmentId = shipmentId;
            OrderItemId = orderItemId; // <-- Yeni
            PackageId = packageId;
            Spec = spec;
            PlannedQuantity = plannedQuantity;
        }

        private ShipmentItem() { }

        // Şoför sahada günceller
        public void UpdateActuals(int loaded, int delivered, int rejected, string? reason)
        {
            LoadedQuantity = loaded;
            DeliveredQuantity = delivered;
            RejectedQuantity = rejected;
            RejectionReason = reason;
        }
    }
}