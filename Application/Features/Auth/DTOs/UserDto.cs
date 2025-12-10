namespace Application.Features.Auth.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; } // AppUserId
        public Guid IdentityId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } // "Transporter", "Supplier", "Corporate".


        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid? WorkerId { get; set; }
    }
}
