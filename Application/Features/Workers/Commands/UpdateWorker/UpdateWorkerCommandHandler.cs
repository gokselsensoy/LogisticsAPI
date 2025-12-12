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
        private readonly IUserRepository _userRepo;
        private readonly IIdentityService _identityService;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkerCommandHandler(
            ICurrentUserService currentUser,
            IWorkerRepository workerRepo,
            IUserRepository userRepo,
            IIdentityService identityService,
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _workerRepo = workerRepo;
            _userRepo = userRepo;
            _identityService = identityService;
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateWorkerCommand request, CancellationToken token)
        {
            // 1. Yetki
            var initiator = await _workerRepo.GetByAppUserIdWithCompanyAsync(_currentUser.UserId, token);
            if (initiator == null || !initiator.Roles.Contains(WorkerRole.Admin))
                throw new UnauthorizedAccessException();

            // 2. Hedef Worker'ı Bul
            var targetWorker = await _workerRepo.GetByIdAsync(request.WorkerId, token);
            if (targetWorker == null || targetWorker.CompanyId != initiator.CompanyId)
                throw new Exception("Çalışan bulunamadı.");

            targetWorker.UpdatePersonalDetails(request.FullName, request.Phone);

            // 4. Worker Güncelleme (Departman, Rol)
            if (targetWorker.DepartmentId != request.DepartmentId)
            {
                // Departman bizim şirkette mi kontrolü yapılmalı
                var dept = await _departmentRepo.GetByIdAsync(request.DepartmentId, token);
                if (dept.CompanyId != initiator.CompanyId) throw new Exception("Geçersiz departman.");

                targetWorker.ChangeDepartment(request.DepartmentId);
            }

            targetWorker.UpdateRoles(request.Roles);

            // 5. Kaydet
            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}
