using Domain.SeedWork;

namespace Domain.Entities
{
    public class ShipmentItem : Entity
    { 
        public Guid PackageId;
        public int Quantity;
    }
}