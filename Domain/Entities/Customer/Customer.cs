using Domain.SeedWork;

namespace Domain.Entities.Customer
{
    public abstract class Customer : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        private readonly List<CustomerAddress> _addresses = new();
        public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();
        protected Customer(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}