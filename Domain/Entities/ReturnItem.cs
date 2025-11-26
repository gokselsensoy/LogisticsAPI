using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities
{
    public class ReturnItem : Entity
    {
        public Guid PackageId { get; private set; }
        public int Quantity { get; private set; }
        public ReturnReason Reason { get; private set; }
    }
}