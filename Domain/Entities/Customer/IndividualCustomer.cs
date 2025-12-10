using Domain.Events;

namespace Domain.Entities.Customer
{
    public class IndividualCustomer : Customer
    {
        public Guid AppUserId { get; private set; }

        private IndividualCustomer() : base(null!, null!, null!) { }

        public IndividualCustomer(string name, Guid appUserId, string email, string phone)
            : base(name, email, phone)
        {
            AppUserId = appUserId;

            AddDomainEvent(new IndividualCustomerRegisteredEvent(this.Id, name, email));
        }
    }
}