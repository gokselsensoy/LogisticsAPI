using Domain.Entities.Departments;
using Domain.Events;
using Domain.Events.RegisterEvents;
using Domain.SeedWork;

namespace Domain.Entities.Company
{
    public class Freelancer : Entity, IAggregateRoot
    {
        public Guid AppUserId { get; private set; }
        public string FullName { get; private set; }
        public string Phone { get; private set; }
        public string? CvrNumber { get; private set; }

        private Freelancer() { }

        public Freelancer(Guid appUserId, string fullName, string phone, string? cvrNumber, string email)
        {
            Id = Guid.NewGuid();
            AppUserId = appUserId;
            FullName = fullName;
            Phone = phone;
            CvrNumber = cvrNumber;

            AddDomainEvent(new FreelancerRegisteredEvent(this.Id, fullName, phone, cvrNumber, email));
        }
    }
}