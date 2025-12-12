using Application.Abstractions.Services;
using Domain.Entities.Departments;
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Departments.Commands.DeleteDepartment
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IWorkerRepository _workerRepo;
        private readonly ICompanyRepository _companyRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDepartmentCommandHandler(
            ICurrentUserService currentUser,
            IWorkerRepository workerRepo,
            ICompanyRepository companyRepo,
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _workerRepo = workerRepo;
            _companyRepo = companyRepo;
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteDepartmentCommand request, CancellationToken token)
        {
            // 1. Yetkiliyi Bul
            var worker = await _workerRepo.GetByAppUserIdWithCompanyAsync(_currentUser.UserId, token);
            if (worker == null || !worker.Roles.Contains(WorkerRole.Admin))
                throw new UnauthorizedAccessException("Departman silme yetkiniz yok.");

            var department = await _departmentRepo.GetByIdAsync(request.DepartmentId, token);
            if (department == null) throw new Exception("Departman bulunamadı.");
            // 3. Domain Logic (Aggregate Root üzerinden silme)
            // Bu metod _departments.Remove() çağırır.
            // EF Core "Deleted" durumu oluşturur.
            _departmentRepo.Delete(department);

            // 4. Kaydet
            // BURADA SİHİR GERÇEKLEŞİR:
            // Interceptor "Deleted" durumunu yakalar.
            // Entity "ISoftDeletable" olduğu için işlemi iptal eder.
            // State = Modified yapar, IsDeleted = true yapar, DeletedAt ve DeletedBy'ı doldurur.
            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}
