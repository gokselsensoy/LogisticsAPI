using Application.Abstractions.Messaging;
using MediatR;

namespace Application.Features.Schedules.Commands.DeleteWeeklyPattern
{
    public class DeleteWeeklyPatternCommand : ICommand<Unit>
    {
        public Guid Id { get; set; }
    }
}
