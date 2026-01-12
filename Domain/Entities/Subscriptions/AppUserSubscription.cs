using Domain.SeedWork;

namespace Domain.Entities.Subscriptions;
public class AppUserSubscription : FullAuditedEntity, IAggregateRoot
{
    public Guid AppUserId { get; set; }
    public Guid SubOrbitProductId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime? ValidUntil { get; set; }
    
    // Navigation Property
    public virtual AppUser AppUser { get; set; }
}
