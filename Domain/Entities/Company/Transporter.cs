using Domain.Events;
using Domain.Events.RegisterEvents;

namespace Domain.Entities.Companies
{
    public class Transporter : Company
    {
        private Transporter() : base(null!, null!, null!) { }

        public Transporter(string name, string cvrNumber, string email, string phone)
            : base(name, cvrNumber, phone)
        {
            AddDomainEvent(new TransporterRegisteredEvent(this.Id, name, email));
        }
    }
}