using Domain.Events.TerminalEvents.Domain.Events;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Departments
{
    public class Terminal : FullAuditedEntity, IAggregateRoot
    {
        public Guid DepartmentId { get; private set; }
        public string Name { get; private set; }
        public Address Address { get; private set; }
        public string? ContactPhone { get; private set; }
        public string? ContactEmail { get; private set; }
        public Guid? ManagerId { get; private set; } // WorkerId

        private Terminal() { }

        public Terminal(Guid departmentId, string name, Address address, string? phone, string? email, Guid? managerId)
        {
            Id = Guid.NewGuid();
            DepartmentId = departmentId;
            Name = name;
            Address = address;
            ContactPhone = phone;
            ContactEmail = email;
            ManagerId = managerId;

            AddDomainEvent(new TerminalCreatedEvent(this.Id));
        }

        // 1. Bilgi Güncelleme
        public void UpdateDetails(string name, string? phone, string? email)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new Exception("Terminal ismi boş olamaz.");
            Name = name;
            ContactPhone = phone;
            ContactEmail = email;
        }

        // 2. Adres Güncelleme
        public void Relocate(Address newAddress)
        {
            if (Address != newAddress) Address = newAddress;
        }

        // 3. Yönetici Atama
        public void AssignManager(Guid? managerId)
        {
            ManagerId = managerId;
        }
    }
}