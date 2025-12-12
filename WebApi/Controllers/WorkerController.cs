using Application.Features.Workers.Commands.CreateWorker;
using Application.Features.Workers.Commands.DeleteWorker;
using Application.Features.Workers.Commands.UpdateWorker;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/workers")]
    [ApiController]
    [Authorize] // Sadece giriş yapmış kullanıcılar
    public class WorkerController : ControllerBase
    {
        private readonly ISender _sender;

        public WorkerController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        // [Authorize(Policy = "AdminOrOwner")] // İstersen Policy ekleyebilirsin
        public async Task<IActionResult> Create([FromBody] CreateWorkerCommand command)
        {
            var workerId = await _sender.Send(command);
            return Ok(new { Id = workerId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWorkerCommand command)
        {
            if (id != command.WorkerId)
                return BadRequest("URL ID ile Body ID uyuşmuyor.");

            await _sender.Send(command);
            return Ok(new { Message = "Çalışan bilgileri güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _sender.Send(new DeleteWorkerCommand { WorkerId = id });
            return Ok(new { Message = "Çalışan silindi (Bağlantı koparıldı)." });
        }
    }
}
