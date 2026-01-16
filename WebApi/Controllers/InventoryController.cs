using Application.Features.Inventories.Commands.AddStock;
using Application.Features.Inventories.Commands.CreateInventory;
using Application.Features.Inventories.Commands.RemoveStock;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    //[Authorize(Roles = "Worker")]
    [Route("api/[controller]")]
    public class InventoryController : ApiControllerBase
    {
        /// <summary>
        /// Depoya yeni bir raf/lokasyon ekler.
        /// </summary>
        [HttpPost("location")]
        public async Task<IActionResult> CreateLocation([FromBody] CreateInventoryCommand command)
        {
            var id = await Mediator.Send(command);
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
            await Mediator.Send(command);
            return Ok(new { Message = "Stok girişi başarılı." });
        }

        /// <summary>
        /// Bir raftan stok düşer (Sevkiyat, Fire).
        /// </summary>
        [HttpPost("stock/out")]
        public async Task<IActionResult> RemoveStock([FromBody] RemoveStockCommand command)
        {
            await Mediator.Send(command);
            return Ok(new { Message = "Stok çıkışı başarılı." });
        }

        // Query Endpointleri (GetInventoryStock, GetHistory vs.) daha sonra eklenebilir.
    }
}
