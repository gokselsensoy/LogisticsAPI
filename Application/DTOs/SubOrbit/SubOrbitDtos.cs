using Application.DTOs.SubOrbit;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NewMultilloApi.Application.DTOs.SubOrbit;

#region Get public catolog
public class PublicProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<PublicPriceDto> Prices { get; set; } = new();
}

public class PublicPriceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public BillingInterval BillingInterval { get; set; }
    public int BillingIntervalCount { get; set; }
    public string Currency { get; set; } = "DKK";
    public Guid ProductId { get; set; }
    public decimal VatRate { get; set; }

}

public enum BillingInterval
{
    Month = 1,
    Year = 2,
    Day = 3
}
#endregion

#region IniateSubscriptionDTOs
public class InitiateSubscriptionDto
{
    [Required(ErrorMessage = "Ödeme yapan adı zorunludur.")]
    public string PayerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string PayerEmail { get; set; } = string.Empty;

    public string PayerExternalId { get; set; } = string.Empty;

    public string? CouponCode { get; set; }

    [Required(ErrorMessage = "PriceId zorunludur.")]
    public Guid PriceId { get; set; }

    [Required(ErrorMessage = "ReturnUrl zorunludur.")]
    [Url(ErrorMessage = "Geçerli bir URL giriniz.")]
    public string ReturnUrl { get; set; } = string.Empty;
}

public class InitiateSubscriptionResponse
{
    public Guid SubscriptionId { get; set; } // Bizim sistemdeki ID
    public string PaymentId { get; set; } = string.Empty; // Nexi ID'si
    public string PaymentPageUrl { get; set; } = string.Empty; // Kullanıcıyı yönlendireceğin link
}

#endregion

#region Webhook Related Models
public class AccessWebhookPayload
{
    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty; // access.granted, access.revoked

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("user")]
    public WebhookUser User { get; set; } = new();

    [JsonPropertyName("access")]
    public WebhookAccessDetails Access { get; set; } = new();

    [JsonPropertyName("features")]
    public List<ProductFeatureMatrixDto> FeaturesData { get; set; } = new();
}

public class WebhookUser
{
    [JsonPropertyName("externalId")]
    public string ExternalId { get; set; } = string.Empty; // Project'in bildiği ID

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}

public class WebhookAccessDetails
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = "Active"; // Active, Suspended

    [JsonPropertyName("planCode")]
    public string PlanCode { get; set; } = string.Empty; // Ürün adı veya kodu
    [JsonPropertyName("planId")]
    public string PlanId { get; set; } = string.Empty; // Ürün adı veya kodu

    [JsonPropertyName("validUntil")]
    public DateTime? ValidUntil { get; set; } // Erişim bitiş tarihi

    [JsonPropertyName("permissions")]
    public List<WebhookPermissionDto> Permissions { get; set; } = new();
}

public class WebhookPermissionDto
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
#endregion

#region CouponValidate
public class ValidateCouponResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountAmount { get; set; }
}

public enum DiscountType
{
    Percentage = 1,   // Yüzde (%)
    FixedAmount = 2   // Sabit Tutar (TL/USD)
}
#endregion