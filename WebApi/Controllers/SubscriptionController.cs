using Application.Abstractions.Services;
using Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewMultilloApi.Application.DTOs.SubOrbit;

namespace WebApi.Controllers;

[Route("api/subscriptions")]
public class SubscriptionController : ApiControllerBase
{
    private readonly ISubOrbitService _subOrbitService;     
    public SubscriptionController(ISubOrbitService subOrbitService)
    {
        _subOrbitService = subOrbitService;
    }
    /// <summary>
    /// Vitrin için aktif abonelik paketlerini listeler.
    /// </summary>
    /// <param name="typeCode">Ürün tipi filtresi (Varsayılan: 600)</param>
    [HttpGet("plans")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPlans([FromQuery] int typeCode = 600)
    {
        var res = await _subOrbitService.GetPublicCatalog(typeCode);
        return Ok(res);
    }

    /// <summary>
    /// İndirim kuponunu doğrular.
    /// </summary>
    /// <param name="code">Kupon Kodu</param>
    /// <param name="productId">Opsiyonel: Belirli bir ürün için geçerlilik kontrolü</param>
    [HttpGet("validate-coupon")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateCoupon([FromQuery] string code, [FromQuery] Guid? productId = null)
    {
        var res = await _subOrbitService.ValidateCouponAsync(code, productId);
        return Ok(res);
    }
}