using Application.Abstractions.Messaging;

namespace Application.Features.Users.Commands.SyncUser
{
    public class SyncUserCommand : ICommand
    {
        public Guid IdentityId { get; set; }
        public string Email { get; set; }
    }
}
