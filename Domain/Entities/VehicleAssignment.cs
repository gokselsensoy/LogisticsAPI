using Domain.SeedWork;

namespace Domain.Entities
{
    // Araç - Sürücü Ataması (Vardiya)
    public class VehicleAssignment : Entity
    {
        public Guid VehicleId { get; private set; }
        public Guid DriverId { get; private set; } // WorkerId (Rolü Driver olan)
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        // gün lazım
    }
}