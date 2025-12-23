using Domain.SeedWork;

namespace Domain.Events.ProductEvents
{
    public record PackageUpdatedEvent(Guid ProductId, Guid PackageId, string PackageName) : IDomainEvent;
}
