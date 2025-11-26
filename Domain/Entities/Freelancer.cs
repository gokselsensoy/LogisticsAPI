using Domain.SeedWork;

namespace Domain.Entities
{
    public class Freelancer : Entity, IAggregateRoot
    {
        public Guid AppUserId { get; private set; } // Kişi bağlantısı
        public string Name { get; private set; }
        public string? CvrNumber { get; private set; } // Şahıs şirketi olabilir

        // Freelancer'ın Aracı (Genelde 1 tanedir ama liste yapalım, esnek olsun)
        private readonly List<Vehicle> _vehicles = new();
        public IReadOnlyCollection<Vehicle> Vehicles => _vehicles.AsReadOnly();

        public Freelancer(Guid appUserId, string name, string? cvrNumber)
        {
            Id = Guid.NewGuid();
            AppUserId = appUserId;
            Name = name;
            CvrNumber = cvrNumber;
        }

        public void AddVehicle(Vehicle vehicle) => _vehicles.Add(vehicle);
    }
}