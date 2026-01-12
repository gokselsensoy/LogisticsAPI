using Application.Features.Products.DTOs;
using Application.Features.Products.Queries;
using Application.Features.Suppliers.DTOs;
using Application.Features.Suppliers.Queries.GetNearbySupplier;
using Application.Shared.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/marketplace")]
    [ApiController]
    public class MarketplaceController : ControllerBase
    {
        private readonly ISender _sender;

        public MarketplaceController(ISender sender)
        {
            _sender = sender;
        }
        /// <summary>
        /// Seçilen konuma hizmet veren Supplier'ları (Aslında Terminalleri) listeler.
        /// Örn: /api/marketplace/suppliers?latitude=39.9&longitude=32.8&searchText=Market
        /// </summary>
        [HttpGet("suppliers")]
        [ProducesResponseType(typeof(PaginatedResponse<SupplierDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNearbySuppliers([FromQuery] GetNearbySuppliersQuery query)
        {
            // [FromQuery] sayesinde URL'deki ?lat=...&long=... parametreleri 
            // otomatik olarak query nesnesine maplenir.
            var result = await _sender.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Seçilen konuma ve filtrelere göre ürünleri listeler.
        /// Örn: /api/marketplace/products?latitude=39.9&longitude=32.8&searchText=Süt&category=Food
        /// </summary>
        [HttpGet("products")]
        [ProducesResponseType(typeof(PaginatedResponse<ProductListingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchProducts([FromQuery] SearchProductsQuery query)
        {
            // Category Enum olduğu için Swagger ve API bunu 
            // int (1,2) veya string ("Food") olarak otomatik çözümler.
            var result = await _sender.Send(query);
            return Ok(result);
        }
    }
}
