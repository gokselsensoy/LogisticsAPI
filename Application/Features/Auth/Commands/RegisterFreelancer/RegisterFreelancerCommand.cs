using Application.Abstractions.Messaging;
using System.Windows.Input;

namespace Application.Features.Auth.Commands.RegisterFreelancer
{
    public class RegisterFreelancerCommand : ICommand<Guid>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }

        public string FullName { get; set; }
        public string? CvrNumber { get; set; }
    }
}
