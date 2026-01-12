using Application.Abstractions.Messaging;
using NewMultilloApi.Application.DTOs.SubOrbit;

namespace Application.Features.Auth.Commands.RegisterSupplier
{
    public class RegisterSupplierCommand : ICommand<InitiateSubscriptionResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string CvrNumber { get; set; }
        public InitiateSubscriptionDto initiateSubscriptionDto { get; set; }
    }
}
