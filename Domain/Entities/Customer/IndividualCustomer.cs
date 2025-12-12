using Domain.Events;

namespace Domain.Entities.Customer
{
    public class IndividualCustomer : Customer
    {
        public Guid AppUserId { get; private set; }

        private IndividualCustomer() : base(null!, null!, null!) { }

        public IndividualCustomer(Guid appUserId, string name, string email, string phone)
            : base(name, email, phone)
        {
            AppUserId = appUserId;
            Name = name;
            Email = email;
            PhoneNumber = phone;

            AddDomainEvent(new IndividualCustomerRegisteredEvent(this.Id, name, email));
        }
    }
}