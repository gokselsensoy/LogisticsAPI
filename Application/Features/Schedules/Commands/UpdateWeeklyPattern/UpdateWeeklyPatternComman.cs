using Application.Abstractions.Messaging;
using MediatR;

namespace Application.Features.Schedules.Commands.UpdateWeeklyPattern
{
    public class UpdateWeeklyPatternCommand : ICommand<Unit>
    {
        public Guid Id { get; set; }
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }
        public Guid? DefaultVehicleId { get; set; }
    }
}
