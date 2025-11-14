using Application.Abstractions.Messaging;

namespace Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommand : ICommand<Guid>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string PhoneNumber { get; set; }
    }
}
