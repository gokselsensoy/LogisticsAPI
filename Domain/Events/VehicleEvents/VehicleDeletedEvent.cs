using Domain.SeedWork;

namespace Domain.Events.VehicleEvents
{
    public record VehicleDeletedEvent(Guid VehicleId, string PlateNumber) : IDomainEvent;
}
