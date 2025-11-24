using Domain.SeedWork;

namespace Domain.Entities
{
    public class Shipment : Entity, IAggregateRoot
    {
        public Guid OrderId { get; private set; }
        // Stoğa göre setlenecek otomatik
        public Guid SourceTerminalId { get; private set; }
        public Location Destination { get; private set; }

        // Atama
        public Guid? AssignedTransporterId { get; private set; }

        public ShipmentStatus Status { get; private set; }

        // Bu shipment içinde hangi paketlerden kaç tane var?
        private readonly List<ShipmentItem> _items = new();
    }
}