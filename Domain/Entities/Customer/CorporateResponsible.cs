using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities.Customer
{
    public class CorporateResponsible : Entity
    {
        public Guid CorporateCustomerId { get; private set; }
        public Guid AppUserId { get; private set; }
        public CorporateRole Role { get; private set; }

        // Şube Haritası
        private readonly List<CorporateAddressResponsibleMap> _assignedAddresses = new();
        public IReadOnlyCollection<CorporateAddressResponsibleMap> AssignedAddresses => _assignedAddresses.AsReadOnly();

        private CorporateResponsible() { }

        public CorporateResponsible(Guid corporateCustomerId, Guid appUserId, CorporateRole role)
        {
            Id = Guid.NewGuid();
            CorporateCustomerId = corporateCustomerId;
            AppUserId = appUserId;
            Role = role;
        }

        public void AssignAddress(Guid addressId)
        {
            if (!_assignedAddresses.Any(x => x.AddressId == addressId))
            {
                _assignedAddresses.Add(new CorporateAddressResponsibleMap(Id, addressId));
            }
        }
    }
}