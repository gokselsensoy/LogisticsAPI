using Application.Features.Baskets.Commands.AddItemToBasket;
using Application.Features.Baskets.Commands.Checkout;
using Application.Features.Baskets.Commands.RemoveItemFromBasket;
using Application.Features.Baskets.Queries.GetMyBasket;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize] // Mutlaka login olunmalı
    [Route("api/[controller]")]
    public class BasketController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetMyBasket()
        {
            // Artık çalışır durumda
            var result = await Mediator.Send(new GetMyBasketQuery());
            return Ok(result);
        }

        // ==========================================
        // SEPET YÖNETİMİ
        // ==========================================

        /// <summary>
        /// Sepete ürün ekler veya miktarını artırır.
        /// </summary>
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddItemToBasketCommand command)
        {
            var basketId = await Mediator.Send(command);
            return Ok(new { BasketId = basketId });
        }

        /// <summary>
        /// Sepetten bir ürünü tamamen siler.
        /// </summary>
        [HttpDelete("items/{packageId}")]
        public async Task<IActionResult> RemoveItem(Guid packageId)
        {
            await Mediator.Send(new RemoveItemFromBasketCommand { PackageId = packageId });
            return NoContent();
        }

        // ==========================================
        // CHECKOUT (SİPARİŞ OLUŞTURMA)
        // ==========================================

        /// <summary>
        /// Sepeti siparişe dönüştürür.
        /// </summary>
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutCommand command)
        {
            // Front-end sepet ID'yi biliyor olabilir ama güvenlik için
            // Handler içinde "User'ın aktif sepetini bul" mantığı daha sağlamdır.
            // Ancak senin CheckoutCommand'inde BasketId var, onu kullanıyoruz.

            var orderId = await Mediator.Send(command);
            return Ok(new { OrderId = orderId });
        }
    }
}
