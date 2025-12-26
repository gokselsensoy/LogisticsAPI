using Application.Abstractions.Messaging;
using MediatR;

namespace Application.Features.CustomerAddresses.Commands.DeleteCustomerAddress
{
    public class DeleteCustomerAddressCommand : ICommand<Unit>
    {
        public Guid AddressId { get; set; }
    }
}
