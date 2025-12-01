using Domain.Enums;
using Domain.SeedWork;
using Domain.ValueObjects;

namespace Domain.Entities.WorkSchedule
{
    public class ScheduleAllocation : Entity
    {
        public Guid DailyWorkScheduleId { get; private set; }

        public TimeRange TimeRange { get; private set; } // Başlangıç - Bitiş

        public AssignmentType Type { get; private set; } // Ne yapıyor?

        // Eğer araç kullanıyorsa burası dolu olur.
        // Depoda çalışıyorsa veya moladaysa boş (null) olur.
        public Guid? VehicleId { get; private set; }

        private ScheduleAllocation() { }

        public ScheduleAllocation(Guid scheduleId, TimeRange timeRange, AssignmentType type, Guid? vehicleId)
        {
            Id = Guid.NewGuid();
            DailyWorkScheduleId = scheduleId;
            TimeRange = timeRange;
            Type = type;
            VehicleId = vehicleId;
        }
    }
}