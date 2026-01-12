using Application.DTOs.SubOrbit;
using NewMultilloApi.Application.DTOs.SubOrbit;

namespace Application.Abstractions.Services;

public interface ISubOrbitService
{
    // Paketleri getirir
    Task<List<PublicProductDto>> GetPublicCatalog(int typeCode);

    Task<List<ProductFeatureMatrixDto>> GetProductFeatureMatrixAsync();

    // Ödeme isteği oluşturur
    Task<InitiateSubscriptionResponse> InitiateCheckoutAsync(InitiateSubscriptionDto request, CancellationToken cancellationToken = default);

    Task<ValidateCouponResponse?> ValidateCouponAsync(string code, Guid? productId = null);

    // Gelen Webhook imzasını doğrular
    bool VerifySignature(string signature, string jsonBody);
}