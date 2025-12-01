using Domain.SeedWork;

namespace Domain.Entities.Customer
{
    public abstract class Customer : Entity, IAggregateRoot
    {
        public string Name { get; protected set; }
        public string Email { get; protected set; }
        public string PhoneNumber { get; protected set; }

        private readonly List<CustomerAddress> _addresses = new();
        public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

        protected Customer() { }

        protected Customer(string name, string email, string phone)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            PhoneNumber = phone;
        }

        public void AddAddress(CustomerAddress address) => _addresses.Add(address);
    }
}