using Domain.Entities.WorkSchedule;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Schedules.Commands.CreateDailySchedule
{
    public class CreateDailyScheduleCommandHandler : IRequestHandler<CreateDailyScheduleCommand, Guid>
    {
        private readonly IDailyWorkScheduleRepository _dailyRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDailyScheduleCommandHandler(IDailyWorkScheduleRepository dailyRepo, IUnitOfWork unitOfWork)
        {
            _dailyRepo = dailyRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateDailyScheduleCommand request, CancellationToken token)
        {
            // Validasyon: O gün zaten kaydı var mı?
            if (await _dailyRepo.ExistsAsync(request.WorkerId, request.Date, token))
                throw new Exception("Bu çalışan için o tarihte zaten bir vardiya kaydı var.");

            var schedule = new DailyWorkSchedule(request.WorkerId, request.Date, request.ShiftStart, request.ShiftEnd);

            _dailyRepo.Add(schedule);
            await _unitOfWork.SaveChangesAsync(token);

            return schedule.Id;
        }
    }
}
