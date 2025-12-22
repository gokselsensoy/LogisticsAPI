using Application.Abstractions.Messaging;
using MediatR;

namespace Application.Features.Schedules.Commands.UpdateDailyShiftTimes
{
    public class UpdateDailyShiftTimesCommand : ICommand<Unit>
    {
        public Guid Id { get; set; } // DailyScheduleId
        public DateTime NewStart { get; set; }
        public DateTime NewEnd { get; set; }
    }
}
