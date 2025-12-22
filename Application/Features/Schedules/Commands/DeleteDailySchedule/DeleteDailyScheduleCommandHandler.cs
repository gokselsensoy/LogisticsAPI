using Domain.Repositories;
using Domain.SeedWork;
using MediatR;
using System.Windows.Input;

namespace Application.Features.Schedules.Commands.DeleteDailySchedule
{
    public class DeleteDailyScheduleCommandHandler : IRequestHandler<DeleteDailyScheduleCommand, Unit>
    {
        private readonly IDailyWorkScheduleRepository _dailyRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDailyScheduleCommandHandler(IDailyWorkScheduleRepository dailyRepo, IUnitOfWork unitOfWork)
        {
            _dailyRepo = dailyRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteDailyScheduleCommand request, CancellationToken token)
        {
            // Sadece ID ile çekmek yeterli (Allocations'a gerek yok, parent silinince onlar da erişilemez olur)
            var schedule = await _dailyRepo.GetByIdAsync(request.Id, token);

            if (schedule == null)
                throw new Exception("Günlük vardiya kaydı bulunamadı.");

            // Soft Delete (Interceptor IsDeleted=true yapacak)
            _dailyRepo.Delete(schedule);

            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
