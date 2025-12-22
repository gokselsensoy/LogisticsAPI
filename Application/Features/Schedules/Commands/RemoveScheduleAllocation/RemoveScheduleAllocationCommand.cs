using Application.Abstractions.Messaging;
using MediatR;

namespace Application.Features.Schedules.Commands.RemoveScheduleAllocation
{
    public class RemoveScheduleAllocationCommand : ICommand<Unit>
    {
        public Guid ScheduleId { get; set; }
        public Guid AllocationId { get; set; }
    }
}
