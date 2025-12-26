using Application.Abstractions.Messaging;
using MediatR;

namespace Application.Features.CustomerAddresses.Commands.AssignAddressToResponsible
{
    public class AssignAddressToResponsibleCommand : ICommand<Unit>
    {
        public Guid TargetResponsibleId { get; set; }
        public Guid AddressId { get; set; }
    }
}
