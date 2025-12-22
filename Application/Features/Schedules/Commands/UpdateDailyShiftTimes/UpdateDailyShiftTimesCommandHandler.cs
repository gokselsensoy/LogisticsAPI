using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Schedules.Commands.UpdateDailyShiftTimes
{
    public class UpdateDailyShiftTimesCommandHandler : IRequestHandler<UpdateDailyShiftTimesCommand, Unit>
    {
        private readonly IDailyWorkScheduleRepository _dailyRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDailyShiftTimesCommandHandler(IDailyWorkScheduleRepository dailyRepo, IUnitOfWork unitOfWork)
        {
            _dailyRepo = dailyRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateDailyShiftTimesCommand request, CancellationToken token)
        {
            // Allocations ile çekiyoruz çünkü vardiya küçültülürse dışarıda kalan görev var mı bakılacak.
            var schedule = await _dailyRepo.GetByIdWithAllocationsAsync(request.Id, token);

            if (schedule == null)
                throw new Exception("Vardiya kaydı bulunamadı.");

            // Domain Entity içindeki metodu çağırıyoruz (Validasyon orada yapılıyor)
            schedule.UpdateShiftTimes(request.NewStart, request.NewEnd);

            _dailyRepo.Update(schedule);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
