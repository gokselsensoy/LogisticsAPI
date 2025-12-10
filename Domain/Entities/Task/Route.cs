using Domain.SeedWork;

namespace Domain.Entities.Task
{
    public class Route : Entity
    {
        public Guid VehicleId { get; private set; }
        public Guid? DriverId { get; private set; }
        public Guid? FreelancerId { get; private set; }
        public DateTime RouteDate { get; private set; }
        // Başlangıç noktası gerekli

        private readonly List<RouteTask> _tasks = new();
        public IReadOnlyCollection<RouteTask> Tasks => _tasks.AsReadOnly();

        private Route() { }
    }
}