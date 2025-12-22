using Application.Abstractions.Messaging;
using Domain.Enums;
using MediatR;

namespace Application.Features.Vehicles.Commands.UpdateVehicle
{
    public class UpdateVehicleCommand : ICommand<Unit>
    {
        public Guid Id { get; set; } // Güncellenecek Araç ID
        public string PlateNumber { get; set; }
        public VehicleType Type { get; set; }
        public double MaxWeightKg { get; set; }
        public double MaxVolumeM3 { get; set; }
        public Guid? DepartmentId { get; set; } // Sadece şirketse değişebilir
    }
}
