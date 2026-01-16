namespace Application.Features.Auth.DTOs;

public class MyProfileDto
{
    public Guid ProfileId { get; set; }
    public Guid AppUserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string ProfileType { get; set; } // "Worker", "IndividualCustomer" vb.

    // Roller (Worker ve CorporateResponsible için önemli)
    public List<string> Roles { get; set; } = new();

    // Şirket Bilgisi (Worker ve CorporateResponsible için)
    // IndividualCustomer ve Freelancer için null dönebilir.
    public OrganizationInfoDto? Organization { get; set; }
}
