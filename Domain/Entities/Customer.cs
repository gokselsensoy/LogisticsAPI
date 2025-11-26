using Domain.SeedWork;

namespace Domain.Entities
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

    // KURUMSAL MÜŞTERİ (Zincir Market/Restoran)
    public class CorporateCustomer : Customer
    {
        public string CvrNumber { get; private set; }

        public CorporateCustomer(string name) : base(name) { }
    }

    public class IndividualCustomer : Customer
    {
        public Guid AppUserId { get; private set; }

        public IndividualCustomer(string name) : base(name) { }
    }
}