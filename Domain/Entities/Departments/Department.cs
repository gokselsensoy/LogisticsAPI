using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Departments
{
    public class Department : FullAuditedEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public Guid CompanyId { get; private set; }
        public Address Address { get; private set; }
        public string? ContactPhone { get; private set; }
        public string? ContactEmail { get; private set; }
        public Guid? ManagerId { get; private set; } // WorkerId
        private readonly List<Terminal> _terminals = new();
        public IReadOnlyCollection<Terminal> Terminals => _terminals.AsReadOnly();
        private Department() { }

        public Department(Guid companyId, string name, Address address, string? contactPhone, string? contactEmail, Guid? managerId)
        {
            Id = Guid.NewGuid();
            CompanyId = companyId;
            Name = name;
            Address = address;
            ContactPhone = contactPhone;
            ContactEmail = contactEmail;
            ManagerId = managerId;
        }

        public void UpdateDetails(string name, string? phone, string? email)
        {
            Name = name;
            ContactPhone = phone;
            ContactEmail = email;
        }

        public void Relocate(Address newAddress)
        {
            if (Address != newAddress)
            {
                Address = newAddress;
            }
        }

        public void AssignManager(Guid? managerId)
        {
            if (ManagerId != managerId)
            {
                ManagerId = managerId;
                // Gerekirse Domain Event: AddDomainEvent(new DepartmentManagerChangedEvent(...));
            }
        }
    }
}