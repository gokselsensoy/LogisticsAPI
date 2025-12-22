using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Vehicles.Commands.UpdateVehicle
{
    public class UpdateVehicleCommandHandler : IRequestHandler<UpdateVehicleCommand, Unit>
    {
        private readonly IVehicleRepository _vehicleRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateVehicleCommandHandler(IVehicleRepository vehicleRepo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _vehicleRepo = vehicleRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateVehicleCommand request, CancellationToken token)
        {
            // 1. Aracı Getir (Silinmemiş olanı)
            var vehicle = await _vehicleRepo.GetByIdActiveAsync(request.Id, token);
            if (vehicle == null) throw new Exception("Araç bulunamadı.");

            // 2. SAHİPLİK KONTROLÜ (OWNERSHIP CHECK)
            if (_currentUser.ProfileType == "Worker")
            {
                // Eğer benim şirketimin ID'si, aracın şirket ID'sine eşit değilse -> HATA
                if (vehicle.CompanyId != _currentUser.CompanyId)
                    throw new UnauthorizedAccessException("Bu aracı güncelleme yetkiniz yok (Farklı Şirket).");
            }
            else if (_currentUser.ProfileType == "Freelancer")
            {
                // Eğer benim FreelancerID'm, aracın FreelancerID'sine eşit değilse -> HATA
                if (vehicle.FreelancerId != _currentUser.ProfileId)
                    throw new UnauthorizedAccessException("Bu aracı güncelleme yetkiniz yok.");
            }
            else
            {
                // Admin vs. değilse ve yukarıdakilere uymuyorsa
                throw new UnauthorizedAccessException("Yetkisiz işlem.");
            }

            // 3. Güncelle
            vehicle.UpdateDetails(
                request.PlateNumber,
                request.Type,
                request.MaxWeightKg,
                request.MaxVolumeM3,
                request.DepartmentId // Sadece şirket aracıysa entity içinde setlenir, yoksa ignore edilir
            );

            // 4. Kaydet
            _vehicleRepo.Update(vehicle);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
