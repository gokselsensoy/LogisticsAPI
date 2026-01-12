using Application.Features.SubOrbit.Commands.PlanFeatureCache;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly ISender _sender;

        public UserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("registerFeatures")]
        public async Task<IActionResult> RegisterFeatures([FromBody] PlanFeatureCacheCommand command)
        {
            await _sender.Send(command);
            return Ok();
        }
    }
}
