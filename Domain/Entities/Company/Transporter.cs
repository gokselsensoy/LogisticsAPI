using Domain.Events;

namespace Domain.Entities.Company
{
    public class Transporter : Company
    {
        private Transporter() : base(null!, null) { }

        public Transporter(string name, string? cvrNumber, string email) : base(name, cvrNumber)
        {
            AddDomainEvent(new TransporterRegisteredEvent(this.Id, name, email));
        }
    }
}