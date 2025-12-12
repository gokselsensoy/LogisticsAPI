using Domain.Enums;
using Domain.Events;

namespace Domain.Entities.Customer
{
    public class CorporateCustomer : Customer
    {
        public string CvrNumber { get; private set; }

        private readonly List<CorporateResponsible> _responsibles = new();
        public IReadOnlyCollection<CorporateResponsible> Responsibles => _responsibles.AsReadOnly();

        private CorporateCustomer() : base(null!, null!, null!) { }

        public CorporateCustomer(string name, string cvrNumber, string email, string phone)
            : base(name, email, phone)
        {
            CvrNumber = cvrNumber;

            AddDomainEvent(new CorporateCustomerRegisteredEvent(this.Id, name, email));
        }

        // Admin ekleme metodu
        public void AddResponsible(Guid appUserId, string fullName, string phone, CorporateRole role)
        {
            _responsibles.Add(new CorporateResponsible(Id, appUserId, fullName, phone, role));
            // AddDomainEvent(new CorporateResponsibleCreatedEvent(responsible.Id, email, ...));
        }
    }
}