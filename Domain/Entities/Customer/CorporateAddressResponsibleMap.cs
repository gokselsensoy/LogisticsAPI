using Domain.SeedWork;

namespace Domain.Entities.Customer
{
    public class CorporateAddressResponsibleMap : Entity
    {
        public Guid ResponsibleId { get; private set; }
        public Guid AddressId { get; private set; }

        public CorporateAddressResponsibleMap(Guid responsibleId, Guid addressId)
        {
            ResponsibleId = responsibleId;
            AddressId = addressId;
        }
    }
}