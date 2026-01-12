namespace Application.Features.Auth.DTOs;

public class LoginResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }

    public List<UserProfileDto> AvailableProfiles { get; set; } = new();
    public bool IsContextSelected { get; set; }
    public SubscriptionProfileDto SubscriptionProfile { get; set; }
    public List<PlanFeatureDto> Features { get; set; }
}
