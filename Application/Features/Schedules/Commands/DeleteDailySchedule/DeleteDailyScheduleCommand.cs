using Application.Abstractions.Messaging;
using MediatR;

namespace Application.Features.Schedules.Commands.DeleteDailySchedule
{
    public class DeleteDailyScheduleCommand : ICommand<Unit>
    {
        public Guid Id { get; set; }
    }
}
