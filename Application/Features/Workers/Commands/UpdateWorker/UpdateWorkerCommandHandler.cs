using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities.Departments;
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Workers.Commands.UpdateWorker
{
    public class UpdateWorkerCommandHandler : IRequestHandler<UpdateWorkerCommand>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IWorkerRepository _workerRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkerCommandHandler(
            ICurrentUserService currentUser,
            IWorkerRepository workerRepo,
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _workerRepo = workerRepo;
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateWorkerCommand request, CancellationToken token)
        {
            // 1. Yetki
            if (!_currentUser.CompanyId.HasValue)
                throw new UnauthorizedAccessException("Bu işlem için bir şirket profili ile giriş yapmalısınız.");

            if (!_currentUser.Roles.Contains("Admin")) // Rol string olarak geliyorsa
                throw new UnauthorizedAccessException("Personel ekleme yetkiniz yok.");

            // 2. Hedef Worker'ı Bul
            var targetWorker = await _workerRepo.GetByIdAsync(request.WorkerId, token);
            if (targetWorker == null || targetWorker.CompanyId != _currentUser.CompanyId.Value)
                throw new Exception("Çalışan bulunamadı.");

            targetWorker.UpdatePersonalDetails(request.FullName, request.Phone);

            // 4. Worker Güncelleme (Departman, Rol)
            if (targetWorker.DepartmentId != request.DepartmentId)
            {
                // Departman bizim şirkette mi kontrolü yapılmalı
                var dept = await _departmentRepo.GetByIdAsync(request.DepartmentId, token);
                if (dept.CompanyId != _currentUser.CompanyId.Value) throw new Exception("Geçersiz departman.");

                targetWorker.ChangeDepartment(request.DepartmentId);
            }

            targetWorker.UpdateRoles(request.Roles);

            // 5. Kaydet
            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}
