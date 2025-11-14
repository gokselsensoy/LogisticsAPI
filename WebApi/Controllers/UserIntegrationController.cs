using Application.Features.Users.Commands.SyncUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/integration")]
    // [ApiKeyAuthorize] // <-- ÇOK ÖNEMLİ: Bu endpoint public olamaz!
    // Burası mutlaka bir API Key, Client Credentials veya IP kısıtlaması ile korunmalı.
    // Şimdilik korumasız bırakıyoruz.
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
            // Gelen isteği doğrudan Application katmanındaki Handler'a yönlendir
            await _sender.Send(command);
            return Ok();
        }
    }
}
