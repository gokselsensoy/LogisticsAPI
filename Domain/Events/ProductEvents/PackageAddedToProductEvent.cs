using Domain.SeedWork;

namespace Domain.Events.ProductEvents
{
    public record PackageAddedToProductEvent(Guid ProductId, Guid PackageId, string PackageName, decimal Price) : IDomainEvent;
}
