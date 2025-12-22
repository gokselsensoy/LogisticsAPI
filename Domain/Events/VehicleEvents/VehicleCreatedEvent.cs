using Domain.SeedWork;

namespace Domain.Events.VehicleEvents
{
    public record VehicleCreatedEvent(Guid VehicleId, string PlateNumber, Guid? CompanyId, Guid? FreelancerId) : IDomainEvent;
}
