using Domain.SeedWork;

namespace Domain.Entities
{
    public class OrderItem : Entity
    {
        public Guid OrderId { get; private set; }
        public Guid PackageId { get; private set; }
        public int Quantity { get; private set; }
    }
}