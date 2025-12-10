using Application.Features.Users.Commands.SyncUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/integration")]
    [Authorize(Policy = "InternalApiAccess")]
    public class UserIntegrationController : ControllerBase
    {
        private readonly ISender _sender; // MediatR

        public UserIntegrationController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("user-sync")]
        public async Task<IActionResult> SyncUser([FromBody] SyncUserCommand command)
        {
            await _sender.Send(command);
            return Ok();
        }
    }
}
