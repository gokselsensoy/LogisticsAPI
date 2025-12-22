using Application.Abstractions.Messaging;
using Domain.Enums;

namespace Application.Features.Vehicles.Commands.CreateVehicle
{
    public class CreateVehicleCommand : ICommand<Guid>
    {
        public string PlateNumber { get; set; }
        public VehicleType Type { get; set; }
        public double MaxWeightKg { get; set; }
        public double MaxVolumeM3 { get; set; }

        // Sadece Şirket Çalışanı (Worker) ise dolu gelmeli, Freelancer ise boş kalabilir.
        public Guid? DepartmentId { get; set; }
    }
}
