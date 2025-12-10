using Application.Abstractions.Messaging;

namespace Application.Features.Auth.Commands.RegisterSupplier
{
    public class RegisterSupplierCommand : ICommand<Guid>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string CvrNumber { get; set; }
    }
}
