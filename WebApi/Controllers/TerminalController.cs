using Application.Features.Inventories.Commands.AddStock;
using Application.Features.Inventories.Commands.CreateInventory;
using Application.Features.Inventories.Commands.RemoveStock;
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

    //[Authorize(Roles = "Worker")]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ISender _sender;

        public InventoryController(ISender sender) => _sender = sender;

        /// <summary>
        /// Depoya yeni bir raf/lokasyon ekler.
        /// </summary>
        [HttpPost("location")]
        public async Task<IActionResult> CreateLocation([FromBody] CreateInventoryCommand command)
        {
            var id = await _sender.Send(command);
            return Ok(new { InventoryId = id });
        }

        // ==========================================
        // STOK HAREKETLERİ (GİRİŞ / ÇIKIŞ)
        // ==========================================

        /// <summary>
        /// Bir rafa stok girişi yapar (Mal Kabul).
        /// </summary>
        [HttpPost("stock/in")]
        public async Task<IActionResult> AddStock([FromBody] AddStockCommand command)
        {
            await _sender.Send(command);
            return Ok(new { Message = "Stok girişi başarılı." });
        }

        /// <summary>
        /// Bir raftan stok düşer (Sevkiyat, Fire).
        /// </summary>
        [HttpPost("stock/out")]
        public async Task<IActionResult> RemoveStock([FromBody] RemoveStockCommand command)
        {
            await _sender.Send(command);
            return Ok(new { Message = "Stok çıkışı başarılı." });
        }

        // Query Endpointleri (GetInventoryStock, GetHistory vs.) daha sonra eklenebilir.
    }
}
