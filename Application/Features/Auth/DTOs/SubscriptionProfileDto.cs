namespace Application.Features.Auth.DTOs;

public class SubscriptionProfileDto
{
    public string PlanName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime? ValidUntil { get; set; }
}
