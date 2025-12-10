using Application.Abstractions.Messaging;

namespace Application.Features.Auth.Commands.RegisterCorporateCustomer
{
    public class RegisterCorporateCustomerCommand : ICommand<Guid>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }

        public string CorporateName { get; set; }
        public string CvrNumber { get; set; }
    }
}
