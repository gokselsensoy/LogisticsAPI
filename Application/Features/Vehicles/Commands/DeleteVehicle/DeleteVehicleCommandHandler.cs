using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Vehicles.Commands.DeleteVehicle
{
    public class DeleteVehicleCommandHandler : IRequestHandler<DeleteVehicleCommand, Unit>
    {
        private readonly IVehicleRepository _vehicleRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteVehicleCommandHandler(IVehicleRepository vehicleRepo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _vehicleRepo = vehicleRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteVehicleCommand request, CancellationToken token)
        {
            // 1. Aracı Getir
            var vehicle = await _vehicleRepo.GetByIdActiveAsync(request.Id, token);
            if (vehicle == null) throw new Exception("Araç bulunamadı veya zaten silinmiş.");

            // 2. SAHİPLİK KONTROLÜ
            if (_currentUser.ProfileType == "Worker")
            {
                if (vehicle.CompanyId != _currentUser.CompanyId)
                    throw new UnauthorizedAccessException("Bu aracı silme yetkiniz yok.");
            }
            else if (_currentUser.ProfileType == "Freelancer")
            {
                if (vehicle.FreelancerId != _currentUser.ProfileId)
                    throw new UnauthorizedAccessException("Bu aracı silme yetkiniz yok.");
            }
            else
            {
                throw new UnauthorizedAccessException("Yetkisiz işlem.");
            }

            _vehicleRepo.Delete(vehicle);

            // 4. Kaydet
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
