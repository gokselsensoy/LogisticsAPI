namespace Domain.Entities.Customer
{
    public class IndividualCustomer : Customer
    {
        public Guid AppUserId { get; private set; }

        public IndividualCustomer(string name) : base(name) { }
    }
}