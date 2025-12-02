using Domain.SeedWork;

namespace Domain.Entities.Task
{
    // Transporter'ın yaptığı günlük plan (Task Creation Ekranı)
    public class DeliveryPlan : Entity, IAggregateRoot
    {
        public Guid? TransporterId { get; private set; }
        public Guid? FreelancerId { get; private set; }
        public DateTime PlanDate { get; private set; }

        // Bir plan birden fazla rotadan oluşur (Her araç için bir rota)
        private readonly List<Route> _routes = new();
        public IReadOnlyCollection<Route> Routes => _routes.AsReadOnly();

        private DeliveryPlan() { }
    }
}