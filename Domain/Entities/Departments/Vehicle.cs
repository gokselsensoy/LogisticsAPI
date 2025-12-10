using Domain.Enums;
using Domain.Exceptions;
using Domain.SeedWork;
using NetTopologySuite.Geometries;

namespace Domain.Entities.Departments
{
    public class Vehicle : Entity, IAggregateRoot
    {
        // --- Ortak Araç Özellikleri ---
        public string PlateNumber { get; private set; }
        public VehicleType Type { get; private set; }
        public double MaxWeightKg { get; private set; }
        public double MaxVolumeM3 { get; private set; }
        public Point? LastKnownLocation { get; private set; }
        public VehicleStatus Status { get; private set; }


        public Guid? CompanyId { get; private set; }
        public Guid? DepartmentId { get; private set; }
        public Guid? FreelancerId { get; private set; }

        private Vehicle() { }

        // Factory 1: Şirket Aracı (Supplier veya Transporter)
        public static Vehicle CreateCompanyVehicle(
            Guid companyId, // SupplierId veya TransporterId buraya gelir
            string plate,
            VehicleType type,
            double weight,
            double volume,
            Guid departmentId)
        {
            if (companyId == Guid.Empty) throw new DomainException("CompanyId boş olamaz.");

            return new Vehicle
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,      // DOLU
                FreelancerId = null,        // BOŞ
                DepartmentId = departmentId,// DOLU
                PlateNumber = plate,
                Type = type,
                MaxWeightKg = weight,
                MaxVolumeM3 = volume,
                Status = VehicleStatus.Active
            };
        }

        // Factory 2: Freelancer Aracı
        public static Vehicle CreateFreelancerVehicle(
            Guid freelancerId,
            string plate,
            VehicleType type,
            double weight,
            double volume)
        {
            if (freelancerId == Guid.Empty) throw new DomainException("FreelancerId boş olamaz.");

            return new Vehicle
            {
                Id = Guid.NewGuid(),
                CompanyId = null,           // BOŞ
                FreelancerId = freelancerId,// DOLU
                DepartmentId = null,        // BOŞ (Freelancer'ın departmanı olmaz)
                PlateNumber = plate,
                Type = type,
                MaxWeightKg = weight,
                MaxVolumeM3 = volume,
                Status = VehicleStatus.Active
            };
        }
    }
}