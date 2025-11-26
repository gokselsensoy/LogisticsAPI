using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities
{
    public class ReturnRequest : Entity, IAggregateRoot
    {
        public Guid CustomerId { get; private set; }
        public Guid? OriginalOrderId { get; private set; }
        public Guid TargetTerminalId { get; private set; }

        public ReturnStatus Status { get; private set; }

        private readonly List<ReturnItem> _items = new();
        public IReadOnlyCollection<ReturnItem> Items => _items.AsReadOnly();
    }
}