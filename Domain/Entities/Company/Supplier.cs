using Domain.Entities.Inventory;
using Domain.Events;
using Domain.Events.RegisterEvents;

namespace Domain.Entities.Company
{
    public class Supplier : Company
    {

        private Supplier() : base(null!, null) { }

        public Supplier(string name, string? cvrNumber, string email) : base(name, cvrNumber)
        {
            AddDomainEvent(new SupplierRegisteredEvent(this.Id, name, email));
        }
    }
}