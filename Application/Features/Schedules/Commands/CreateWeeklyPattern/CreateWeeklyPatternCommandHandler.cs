using Domain.Entities.WorkSchedule;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Schedules.Commands.CreateWeeklyPattern
{
    public class CreateWeeklyPatternCommandHandler : IRequestHandler<CreateWeeklyPatternCommand, Guid>
    {
        private readonly IWeeklyShiftPatternRepository _patternRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CreateWeeklyPatternCommandHandler(IWeeklyShiftPatternRepository patternRepo, IUnitOfWork unitOfWork)
        {
            _patternRepo = patternRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateWeeklyPatternCommand request, CancellationToken token)
        {
            // Validasyon: Aynı gün için zaten şablon var mı?
            var existing = await _patternRepo.GetByWorkerAndDayAsync(request.WorkerId, request.DayOfWeek, token);
            if (existing != null)
                throw new Exception("Bu çalışan için seçilen günde zaten bir vardiya şablonu var.");

            var pattern = new WeeklyShiftPattern(
                request.WorkerId,
                request.DayOfWeek,
                request.ShiftStart,
                request.ShiftEnd,
                request.DefaultVehicleId
            );

            _patternRepo.Add(pattern);
            await _unitOfWork.SaveChangesAsync(token);

            return pattern.Id;
        }
    }
}
