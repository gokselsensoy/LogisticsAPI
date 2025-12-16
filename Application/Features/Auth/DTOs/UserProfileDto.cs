namespace Application.Features.Auth.DTOs
{
    public class UserProfileDto
    {
        public string ProfileType { get; set; } // "Worker", "Freelancer", "IndividualCustomer"
        public Guid ProfileId { get; set; } // O tablodaki ID'si (WorkerId vb.)
        public Guid? CompanyId { get; set; }
        public string Name { get; set; } // Şirket ismi veya Şahıs ismi
        public List<string> Roles { get; set; } = new();
    }
}
