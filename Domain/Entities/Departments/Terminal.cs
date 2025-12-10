using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Departments
{
    public class Terminal : Entity, IAggregateRoot
    {
        public string DepartmentId { get; private set; }
        public string Name { get; private set; }
        public Address Address { get; private set; }
        public string? ContactPhone { get; private set; }
        public string? ContactEmail { get; private set; }

        private Terminal() { }

        public Terminal(string name, Address address)
        {
            Id = Guid.NewGuid();
            Name = name;
            Address = address;
        }
    }
}