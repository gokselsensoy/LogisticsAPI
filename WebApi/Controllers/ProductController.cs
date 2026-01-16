using Application.Features.Products.Commands.AddPackage;
using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Products.Commands.DeletePackage;
using Application.Features.Products.Commands.DeleteProduct;
using Application.Features.Products.Commands.UpdatePackage;
using Application.Features.Products.Commands.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    //[Authorize(Roles = "Supplier")] // Sadece Tedarikçiler erişebilir (Policy de olabilir)
    [Route("api/[controller]")]
    public class ProductController : ApiControllerBase
    {
        /// <summary>
        /// Yeni bir ana ürün oluşturur (Örn: Coca Cola Zero).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var productId = await Mediator.Send(command);
            return Ok(new { ProductId = productId });
        }

        /// <summary>
        /// Ürün bilgilerini (İsim, Açıklama) günceller.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Ürünü ve ona bağlı tüm paketleri siler (Soft Delete).
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            await Mediator.Send(new DeleteProductCommand { Id = id });
            return NoContent();
        }

        /// <summary>
        /// Bir ürüne yeni paket tipi ekler (Örn: 6'lı Koli).
        /// </summary>
        [HttpPost("packages")]
        public async Task<IActionResult> AddPackage([FromBody] AddPackageCommand command)
        {
            var packageId = await Mediator.Send(command);
            return Ok(new { PackageId = packageId });
        }

        /// <summary>
        /// Mevcut bir paketin fiyat veya boyut bilgilerini günceller.
        /// </summary>
        [HttpPut("packages")]
        public async Task<IActionResult> UpdatePackage([FromBody] UpdatePackageCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Bir üründen spesifik bir paketi siler.
        /// </summary>
        [HttpDelete("{productId}/packages/{packageId}")]
        public async Task<IActionResult> DeletePackage(Guid productId, Guid packageId)
        {
            await Mediator.Send(new DeletePackageCommand { ProductId = productId, PackageId = packageId });
            return NoContent();
        }
    }
}
