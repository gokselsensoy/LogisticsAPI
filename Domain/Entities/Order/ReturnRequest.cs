using Domain.Enums;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Order
{
    public class ReturnRequest : Entity, IAggregateRoot
    {
        public Guid? CustomerId { get; private set; }
        public ContactInfo Contact { get; private set; }

        // Hangi siparişten? (Zorunlu değil, sadece boş kasa iadesi olabilir)
        public Guid? OriginalOrderId { get; private set; }
        public Address PickupLocation { get; private set; }

        public Guid TargetTerminalId { get; private set; }

        public ReturnStatus Status { get; private set; }
        public Guid SupplierId { get; private set; }

        private readonly List<ReturnItem> _items = new();
        public IReadOnlyCollection<ReturnItem> Items => _items.AsReadOnly();

        public Guid? CreatedShipmentId { get; private set; }

        private ReturnRequest() { }
    }
}