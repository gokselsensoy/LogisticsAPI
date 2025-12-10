using Domain.SeedWork;

namespace Domain.Events
{
    public record TransporterRegisteredEvent(Guid CompanyId, string Name, string Email) : IDomainEvent;
}
