using Domain.Events.RegisterEvents;

namespace Domain.Entities.Companies
{
    public class Supplier : Company
    {

        private Supplier() : base(null!, null!, null!) { }

        public Supplier(string name, string cvrNumber, string email, string phone)
            : base(name, cvrNumber, phone)
        {
            AddDomainEvent(new SupplierRegisteredEvent(this.Id, name, email));
        }
    }
}