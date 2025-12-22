using Application.Abstractions.Messaging;

namespace Application.Features.Schedules.Commands.CreateDailySchedule
{
    public class CreateDailyScheduleCommand : ICommand<Guid>
    {
        public Guid WorkerId { get; set; }
        public DateOnly Date { get; set; }
        public DateTime ShiftStart { get; set; } // 2023-10-25 08:00
        public DateTime ShiftEnd { get; set; }   // 2023-10-25 18:00
    }
}
