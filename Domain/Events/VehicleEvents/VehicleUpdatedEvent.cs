using Domain.SeedWork;

namespace Domain.Events.VehicleEvents
{
    public record VehicleUpdatedEvent(Guid VehicleId, string PlateNumber) : IDomainEvent;
}
