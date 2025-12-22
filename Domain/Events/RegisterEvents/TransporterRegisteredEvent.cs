using Domain.SeedWork;

namespace Domain.Events.RegisterEvents
{
    public record TransporterRegisteredEvent(Guid CompanyId, string Name, string Email) : IDomainEvent;
}
