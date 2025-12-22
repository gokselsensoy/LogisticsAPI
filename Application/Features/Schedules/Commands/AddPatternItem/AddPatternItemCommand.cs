using Application.Abstractions.Messaging;
using Domain.Enums;
using MediatR;

namespace Application.Features.Schedules.Commands.AddPatternItem
{
    public class AddPatternItemCommand : ICommand<Unit>
    {
        public Guid PatternId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AssignmentType Type { get; set; } // Driving, Break, WarehouseWork
        public Guid? DefaultVehicleId { get; set; }
    }
}
