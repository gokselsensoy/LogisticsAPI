using Domain.SeedWork;

namespace Domain.Entities.Customer
{
    public class CorporateAddressResponsibleMap : Entity
    {
        public Guid ResponsibleId { get; private set; }
        public Guid AddressId { get; private set; }

        private CorporateAddressResponsibleMap() { }

        public CorporateAddressResponsibleMap(Guid responsibleId, Guid addressId)
        {
            ResponsibleId = responsibleId;
            AddressId = addressId;
        }
    }
}