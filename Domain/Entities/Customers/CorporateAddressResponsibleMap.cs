using Domain.SeedWork;

namespace Domain.Entities.Customers
{
    public class CorporateAddressResponsibleMap : Entity
    {
        public Guid ResponsibleId { get; private set; }
        public Guid AddressId { get; private set; }

        public CustomerAddress CustomerAddress { get; private set; }
        public CorporateResponsible Responsible { get; private set; }
        private CorporateAddressResponsibleMap() { }

        public CorporateAddressResponsibleMap(Guid responsibleId, Guid addressId)
        {
            ResponsibleId = responsibleId;
            AddressId = addressId;
        }
    }
}