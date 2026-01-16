using Application.Features.SubOrbit.Commands.PlanFeatureCache;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/user")]
    public class UserController : ApiControllerBase
    {
        [HttpPost("registerFeatures")]
        public async Task<IActionResult> RegisterFeatures([FromBody] PlanFeatureCacheCommand command)
        {
            await Mediator.Send(command);
            return Ok();
        }
    }
}
