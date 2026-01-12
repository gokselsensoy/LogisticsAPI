using Domain.SeedWork;

namespace Domain.Entities.Subscriptions;
public class PlanFeatureCache : FullAuditedEntity, IAggregateRoot
{
    public Guid SubOrbitProductId { get; set; }
    public Guid FeatureId { get; set; }
    public string FeatureCode { get; set; } = string.Empty; // invoice_create
    public string FeatureName { get; set; } = string.Empty; // Fatura Oluşturma
}
