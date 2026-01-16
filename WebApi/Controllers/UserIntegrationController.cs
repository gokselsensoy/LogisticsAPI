using Application.Features.Users.Commands.SyncUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/integration")]
    [Authorize(Policy = "InternalApiAccess")]
    public class UserIntegrationController : ApiControllerBase
    {
        [HttpPost("user-sync")]
        public async Task<IActionResult> SyncUser([FromBody] SyncUserCommand command)
        {
            await Mediator.Send(command);
            return Ok();
        }
    }
}
