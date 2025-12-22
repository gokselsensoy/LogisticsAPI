using Application.Abstractions.Services;
using Domain.Entities.Departments;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Vehicles.Commands.CreateVehicle
{
    public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Guid>
    {
        private readonly IVehicleRepository _vehicleRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CreateVehicleCommandHandler(
            IVehicleRepository vehicleRepo,
            ICurrentUserService currentUser,
            IUnitOfWork unitOfWork)
        {
            _vehicleRepo = vehicleRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateVehicleCommand request, CancellationToken token)
        {
            // 1. Plaka Unique Kontrolü
            if (await _vehicleRepo.IsPlateExistsAsync(request.PlateNumber, token))
                throw new Exception($"Bu plakaya ({request.PlateNumber}) sahip bir araç zaten var.");

            Vehicle vehicle;

            // 2. Profil Tipine Göre Oluşturma
            if (_currentUser.ProfileType == "Worker")
            {
                // --- WORKER / ŞİRKET SENARYOSU ---
                if (!_currentUser.CompanyId.HasValue)
                    throw new UnauthorizedAccessException("Şirket bilgisi bulunamadı.");

                if (!request.DepartmentId.HasValue || request.DepartmentId == Guid.Empty)
                    throw new Exception("Şirket araçları için Departman seçimi zorunludur.");

                // Token'daki CompanyId kullanılıyor (Güvenlik için)
                vehicle = Vehicle.CreateCompanyVehicle(
                    _currentUser.CompanyId.Value,
                    request.DepartmentId.Value,
                    request.PlateNumber,
                    request.Type,
                    request.MaxWeightKg,
                    request.MaxVolumeM3
                );
            }
            else if (_currentUser.ProfileType == "Freelancer")
            {
                // --- FREELANCER SENARYOSU ---
                if (!_currentUser.ProfileId.HasValue)
                    throw new UnauthorizedAccessException("Freelancer profili bulunamadı.");

                // Token'daki ProfileId (FreelancerId) kullanılıyor
                vehicle = Vehicle.CreateFreelancerVehicle(
                    _currentUser.ProfileId.Value,
                    request.PlateNumber,
                    request.Type,
                    request.MaxWeightKg,
                    request.MaxVolumeM3
                );
            }
            else
            {
                throw new UnauthorizedAccessException("Araç ekleme yetkiniz yok (Geçersiz Profil).");
            }

            // 3. Kaydet
            _vehicleRepo.Add(vehicle);
            await _unitOfWork.SaveChangesAsync(token);

            return vehicle.Id;
        }
    }
}
