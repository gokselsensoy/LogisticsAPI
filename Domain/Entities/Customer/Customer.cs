using Domain.Enums;
using Domain.Exceptions;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Customer
{
    public abstract class Customer : FullAuditedEntity, IAggregateRoot
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

        public CustomerAddress AddAddress(string title, Address address, AddressType type)
        {
            var newAddress = new CustomerAddress(Id, title, address, type);
            _addresses.Add(newAddress);
            return newAddress;
        }

        public void UpdateAddress(Guid addressId, string title, Address address, AddressType type)
        {
            var existing = _addresses.FirstOrDefault(x => x.Id == addressId);
            if (existing == null) throw new DomainException("Adres bulunamadı.");

            existing.UpdateDetails(title, address, type);
        }

        public void RemoveAddress(Guid addressId)
        {
            var existing = _addresses.FirstOrDefault(x => x.Id == addressId);
            if (existing == null) throw new DomainException("Adres bulunamadı.");

            // Soft Delete mantığı (FullAuditedEntity ise)
            // Repository üzerinden Remove çağrıldığında Interceptor halleder
            // Ancak Listeden çıkarmak EF Core'da delete anlamına gelir (Orphan Removal varsa).
            _addresses.Remove(existing);
        }
    }
}