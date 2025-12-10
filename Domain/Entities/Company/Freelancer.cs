using Domain.Entities.Departments;
using Domain.Events;
using Domain.SeedWork;

namespace Domain.Entities.Company
{
    public class Freelancer : Entity, IAggregateRoot
    {
        public Guid AppUserId { get; private set; }
        public string Name { get; private set; }
        public string? CvrNumber { get; private set; }

        private readonly List<Vehicle> _vehicles = new();
        public IReadOnlyCollection<Vehicle> Vehicles => _vehicles.AsReadOnly();

        private Freelancer() { }

        public Freelancer(Guid appUserId, string name, string? cvrNumber, string email)
        {
            Id = Guid.NewGuid();
            AppUserId = appUserId;
            Name = name;
            CvrNumber = cvrNumber;

            AddDomainEvent(new FreelancerRegisteredEvent(this.Id, name, email));
        }

        public void AddVehicle(Vehicle vehicle) => _vehicles.Add(vehicle);
    }
}