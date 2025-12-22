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
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly ISender _sender;

        public VehicleController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVehicleCommand command)
        {
            var vehicleId = await _sender.Send(command);
            return Ok(new { VehicleId = vehicleId });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateVehicleCommand command)
        {
            await _sender.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _sender.Send(new DeleteVehicleCommand { Id = id });
            return NoContent();
        }
    }
}
