using Domain.Enums;
using Domain.Events.VehicleEvents;
using Domain.Exceptions;
using Domain.SeedWork;
using NetTopologySuite.Geometries;

namespace Domain.Entities.Departments
{
    public class Vehicle : FullAuditedEntity, IAggregateRoot
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

        public static Vehicle CreateCompanyVehicle(
                    Guid companyId,
                    Guid departmentId,
                    string plate,
                    VehicleType type,
                    double weight,
                    double volume)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId boş olamaz.");
            if (departmentId == Guid.Empty) throw new ArgumentException("DepartmentId boş olamaz.");

            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                DepartmentId = departmentId,
                FreelancerId = null,
                PlateNumber = plate,
                Type = type,
                MaxWeightKg = weight,
                MaxVolumeM3 = volume,
                Status = VehicleStatus.Active,
                IsDeleted = false
            };

            vehicle.AddDomainEvent(new VehicleCreatedEvent(vehicle.Id, plate, companyId, null));
            return vehicle;
        }

        // 2. Freelancer Aracı Oluşturma
        public static Vehicle CreateFreelancerVehicle(
            Guid freelancerId,
            string plate,
            VehicleType type,
            double weight,
            double volume)
        {
            if (freelancerId == Guid.Empty) throw new ArgumentException("FreelancerId boş olamaz.");

            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                CompanyId = null,
                DepartmentId = null,
                FreelancerId = freelancerId,
                PlateNumber = plate,
                Type = type,
                MaxWeightKg = weight,
                MaxVolumeM3 = volume,
                Status = VehicleStatus.Active,
                IsDeleted = false
            };

            vehicle.AddDomainEvent(new VehicleCreatedEvent(vehicle.Id, plate, null, freelancerId));
            return vehicle;
        }

        // --- UPDATE METHOD ---
        public void UpdateDetails(string plate, VehicleType type, double weight, double volume, Guid? departmentId = null)
        {
            PlateNumber = plate;
            Type = type;
            MaxWeightKg = weight;
            MaxVolumeM3 = volume;

            // Eğer şirket aracıysa departman güncellenebilir
            if (CompanyId.HasValue && departmentId.HasValue && departmentId != Guid.Empty)
            {
                DepartmentId = departmentId.Value;
            }

            AddDomainEvent(new VehicleUpdatedEvent(Id, PlateNumber));
        }

        public void UpdateLocation(Point location)
        {
            LastKnownLocation = location;
            // Buraya event eklenmeyebilir, çok sık tetikleneceği için sistemi yorabilir.
        }

        public void ChangeStatus(VehicleStatus status)
        {
            Status = status;
        }
    }
}