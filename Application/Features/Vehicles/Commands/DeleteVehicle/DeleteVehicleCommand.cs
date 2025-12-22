using Application.Abstractions.Messaging;
using MediatR;

namespace Application.Features.Vehicles.Commands.DeleteVehicle
{
    public class DeleteVehicleCommand : ICommand<Unit>
    {
        public Guid Id { get; set; }
    }
}
