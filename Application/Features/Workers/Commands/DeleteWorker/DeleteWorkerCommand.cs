using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Workers.Commands.DeleteWorker
{
    public class DeleteWorkerCommand : ICommand
    {
        public Guid WorkerId { get; set; }
    }

    public class DeleteWorkerCommandHander : IRequest<DeleteWorkerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWorkerRepository _workerRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepository _userRepo;

        public DeleteWorkerCommandHander(IUnitOfWork unitOfWork, IWorkerRepository workerRepository, ICurrentUserService currentUser, IUserRepository userRepo)
        {
            _unitOfWork = unitOfWork;
            _workerRepo = workerRepository;
            _currentUser = currentUser;
            _userRepo = userRepo;
        }

        public async Task Handle(DeleteWorkerCommand request, CancellationToken token)
        {
            // 1. Yetki (Sadece Admin)
            var initiator = await _workerRepo.GetByAppUserIdWithCompanyAsync(_currentUser.UserId, token);
            if (initiator == null || !initiator.Roles.Contains(WorkerRole.Admin))
                throw new UnauthorizedAccessException();

            // 2. Hedef Worker
            var targetWorker = await _workerRepo.GetByIdAsync(request.WorkerId, token);
            if (targetWorker == null || targetWorker.CompanyId != initiator.CompanyId)
                throw new Exception("Çalışan bulunamadı.");

            // 3. AppUser'ı Bul
            var appUser = await _userRepo.GetByIdAsync(targetWorker.AppUserId, token);

            // B. Local AppUser'ı Güncelle (Unique Index hatası almamak için)
            // Eğer AppUser tablosunda Email üzerinde Unique Index varsa, email'i değiştirmeliyiz.
            // "ahmet@mail.com" -> "deleted_GUID@mail.com"
            targetWorker.UnlinkUser();

            // 4. Worker Soft Delete
            // Repository.Delete çağırdığımızda Interceptor araya girip IsDeleted=true yapacak.
            // Worker kaydı duracak (Raporlar için), ama email boşa çıktığı için aynı maille yeni kayıt açılabilecek.
            _workerRepo.Delete(targetWorker);

            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}
