using Application.Abstractions.Messaging;

namespace Application.Features.Schedules.Commands.CreateWeeklyPattern
{
    public class CreateWeeklyPatternCommand : ICommand<Guid>
    {
        public Guid WorkerId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan ShiftStart { get; set; } // "08:00:00"
        public TimeSpan ShiftEnd { get; set; }   // "18:00:00"
        public Guid? DefaultVehicleId { get; set; }
    }
}
