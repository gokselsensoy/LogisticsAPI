using Domain.SeedWork;

namespace Domain.Events.TerminalEvents
{
    namespace Domain.Events
    {
        public record TerminalCreatedEvent(Guid TerminalId) : IDomainEvent;
    }
}
