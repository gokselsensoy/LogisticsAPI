using Application.Abstractions.Messaging;
using Domain.Enums;
using MediatR;

namespace Application.Features.Schedules.Commands.AddScheduleAllocation
{
    public class AddScheduleAllocationCommand : ICommand<Unit>
    {
        public Guid ScheduleId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AssignmentType Type { get; set; }
        public Guid? VehicleId { get; set; }
    }
}
