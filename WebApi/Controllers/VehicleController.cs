using Application.Features.Vehicles.Commands.CreateVehicle;
using Application.Features.Vehicles.Commands.DeleteVehicle;
using Application.Features.Vehicles.Commands.UpdateVehicle;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize] // Token zorunlu
    [Route("api/[controller]")]
    public class VehicleController : ApiControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVehicleCommand command)
        {
            var vehicleId = await Mediator.Send(command);
            return Ok(new { VehicleId = vehicleId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateVehicleCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Mediator.Send(new DeleteVehicleCommand { Id = id });
            return NoContent();
        }
    }
}
