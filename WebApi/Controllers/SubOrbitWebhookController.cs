using Application.Abstractions.Services;
using Application.Features.SubOrbit.Commands.ProcessWebhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewMultilloApi.Application.DTOs.SubOrbit;
using System.Text.Json;

namespace WebApi.Controllers;

[Route("api/webhooks/suborbit")]
public class SubOrbitWebhookController : ApiControllerBase
{
    private readonly ISubOrbitService _subOrbitService;
    private readonly ILogger<SubOrbitWebhookController> _logger;

    public SubOrbitWebhookController(
        ISubOrbitService subOrbitService,
        ILogger<SubOrbitWebhookController> logger)
    {
        _subOrbitService = subOrbitService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        // 1. Header Kontrolü
        if (!Request.Headers.TryGetValue("X-SubOrbit-Signature", out var signature))
        {
            _logger.LogWarning("Webhook isteği reddedildi: İmza (Signature) bulunamadı.");
            return Unauthorized();
        }

        // 2. Body Okuma (Doğrulama için ham string hali gerekli)
        using var reader = new StreamReader(Request.Body);
        var jsonBody = await reader.ReadToEndAsync();

        // 3. Güvenlik Doğrulaması (Faz 1'de yazdığımız servis)
        if (!_subOrbitService.VerifySignature(signature.ToString(), jsonBody))
        {
            _logger.LogWarning("Webhook isteği reddedildi: Geçersiz İmza!");
            return Unauthorized("Invalid Signature");
        }

        // 4. İşleme (Parsing & Dispatching)
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var payload = JsonSerializer.Deserialize<AccessWebhookPayload>(jsonBody, options);

            if (payload == null)
                return BadRequest("Payload boş veya hatalı format.");

            // MediatR ile Application katmanına gönder
            await Mediator.Send(new ProcessWebhookCommand(payload));

            // SubOrbit'e "Tamam, aldım" mesajı
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook işlenirken kritik hata.");
            return StatusCode(500);
        }
    }
}