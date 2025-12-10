using Application.Abstractions.Messaging;

namespace Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : ICommand<Guid>
    {
        public string Name { get; set; }

        // Opsiyonel İletişim Bilgileri
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid? ManagerId { get; set; } // Yönetici Atanacaksa

        // --- ADRES BİLGİLERİ (Zorunlu) ---
        public string Street { get; set; }      // Örn: Østerbrogade
        public string BuildingNo { get; set; }  // Örn: 12A
        public string ZipCode { get; set; }     // Örn: 2100
        public string City { get; set; }        // Örn: København
        public string? Country { get; set; }    // Varsayılan DK

        // --- ADRES DETAYLARI (Opsiyonel) ---
        public string? Floor { get; set; }      // Örn: "st", "1", "kl"
        public string? Door { get; set; }       // Örn: "th", "tv"

        // --- KONUM (Harita için) ---
        public double Latitude { get; set; }    // Enlem
        public double Longitude { get; set; }   // Boylam
    }
}
