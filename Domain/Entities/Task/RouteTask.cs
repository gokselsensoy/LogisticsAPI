using Domain.Enums;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.Task
{
    public class RouteTask : Entity
    {
        public Guid RouteId { get; private set; }
        public int Sequence { get; private set; }
        public TaskType Type { get; private set; }
        public Address TargetLocation { get; private set; }

        // Görev neyle ilgili?
        public Guid? OrderId { get; private set; } // Teslimat ise
        public Guid? ReturnRequestId { get; private set; } // İade ise

        private RouteTask() { }
    }
}