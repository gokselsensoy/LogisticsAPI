using Application.Features.Terminals.Commands.CreateTerminal;
using Application.Features.Terminals.Commands.DeleteTerminal;
using Application.Features.Terminals.Commands.UpdateTerminal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/terminals")]
    [ApiController]
    [Authorize]
    public class TerminalController : ControllerBase
    {
        private readonly ISender _sender;

        public TerminalController(ISender sender) => _sender = sender;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTerminalCommand command)
        {
            return Ok(new { Id = await _sender.Send(command) });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTerminalCommand command)
        {
            if (id != command.TerminalId) return BadRequest();
            await _sender.Send(command);
            return Ok(new { Message = "Terminal güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _sender.Send(new DeleteTerminalCommand { TerminalId = id });
            return Ok(new { Message = "Terminal silindi." });
        }
    }
}
