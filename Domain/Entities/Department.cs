using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Department : Entity
    {
        public string Name { get; private set; }
        public Guid CompanyId { get; private set; }
        public Address Address { get; private set; }
        public string? ContactPhone { get; private set; }
        public string? ContactEmail { get; private set; }
        public Guid? ManagerId { get; private set; } // WorkerId
        private Department() { }

        public Department(string name, Guid companyId, Address address)
        {
            Id = Guid.NewGuid();
            Name = name;
            CompanyId = companyId;
            Address = address;
        }

        // Lokasyon güncelleme (Örn: Departman taşındı)
        public void Relocate(Address newAddress)
        {
            Address = newAddress;
            // Burada Domain Event fırlatılabilir: DepartmentRelocatedEvent
        }
    }
}