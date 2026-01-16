namespace Application.Features.Auth.DTOs;

public class OrganizationInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; } // "Transporter", "Supplier", "CorporateCustomer"
    public bool IsAdmin { get; set; } // Kullanıcının o şirketteki yetkisi
}
