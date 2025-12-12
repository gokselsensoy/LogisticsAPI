using Domain.SeedWork;

namespace Domain.Events.WorkerEvents
{
    public record WorkerCreatedEvent(Guid WorkerId, string Email, string FullName, string Phone, string TempPassword) : IDomainEvent;
}
