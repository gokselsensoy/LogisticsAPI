using Domain.Enums;
using Domain.Events;

namespace Domain.Entities.Customer
{
    public class CorporateCustomer : Customer
    {
        public string CvrNumber { get; private set; }

        private CorporateCustomer() : base(null!, null!, null!) { }

        public CorporateCustomer(string name, string cvrNumber, string email, string phone)
            : base(name, email, phone)
        {
            CvrNumber = cvrNumber;

            AddDomainEvent(new CorporateCustomerRegisteredEvent(this.Id, name, email));
        }

        public void UpdateCvr(string newCvr)
        {
            CvrNumber = newCvr;
        }

        public CorporateResponsible CreateResponsible(Guid appUserId, string fullName, string phone, List<CorporateRole> roles)
        {
            // Burada 'this.Id' diyerek Sorumlu'yu bu şirkete bağlıyoruz.
            // CorporateResponsible constructor'ı (companyId, appUserId, name...) sırasında olmalı.
            return new CorporateResponsible(this.Id, appUserId, fullName, phone, roles);
        }
    }
}