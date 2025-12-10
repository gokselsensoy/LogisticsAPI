using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities.WorkSchedule
{
    public class ShiftPatternItem : Entity
    {
        public Guid WeeklyShiftPatternId { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public AssignmentType Type { get; private set; }
        public Guid? DefaultVehicleId { get; private set; } // Bu saat aralığındaki varsayılan araç

        private ShiftPatternItem() { }

        public ShiftPatternItem(Guid patternId, TimeSpan start, TimeSpan end, AssignmentType type, Guid? vehicleId)
        {
            WeeklyShiftPatternId = patternId;
            StartTime = start;
            EndTime = end;
            Type = type;
            DefaultVehicleId = vehicleId;
        }
    }
}