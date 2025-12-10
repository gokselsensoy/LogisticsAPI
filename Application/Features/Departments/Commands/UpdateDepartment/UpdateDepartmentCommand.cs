using Application.Abstractions.Messaging;

namespace Application.Features.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommand : ICommand
    {
        public Guid DepartmentId { get; set; }

        // --- GÜNCELLENEBİLİR ALANLAR ---
        public string Name { get; set; }        // Zorunlu (Frontend dolu yollamalı)
        public string? Phone { get; set; }      // Değişebilir
        public string? Email { get; set; }      // Değişebilir
        public Guid? ManagerId { get; set; }    // Değişebilir

        // --- ADRES BİLGİLERİ (Tam Paket) ---
        // Adres Value Object olduğu için en ufak değişimde bile tamamı yeniden oluşturulur.
        public string Street { get; set; }
        public string BuildingNo { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string? Country { get; set; }

        // Detaylar
        public string? Floor { get; set; }
        public string? Door { get; set; }

        // Konum
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
