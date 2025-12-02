using Domain.Enums;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Customer
{
    public class CustomerAddress : Entity
    {
        public Guid CustomerId { get; private set; }
        public string Title { get; private set; } // "Kadıköy Şubesi"
        public Address Address { get; private set; } // Koordinat
        public AddressType AddressType { get; private set; }

        private CustomerAddress() { }

        public CustomerAddress(Guid customerId, string title, Address address)
        {
            CustomerId = customerId;
            Title = title;
            Address = address;
        }
    }
}