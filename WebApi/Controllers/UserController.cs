using Application.Features.Users.Commands.RegisterUser;
using Application.Features.Users.Commands.SyncUser;
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

        [HttpPost("register")]
        public async Task<IActionResult> SyncUser([FromBody] RegisterUserCommand command)
        {
            await _sender.Send(command);
            return Ok();
        }
    }
}
