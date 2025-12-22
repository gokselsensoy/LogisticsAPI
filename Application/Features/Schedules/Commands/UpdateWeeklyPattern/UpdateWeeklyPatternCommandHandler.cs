using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Schedules.Commands.UpdateWeeklyPattern
{
    public class UpdateWeeklyPatternCommandHandler : IRequestHandler<UpdateWeeklyPatternCommand, Unit>
    {
        private readonly IWeeklyShiftPatternRepository _patternRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWeeklyPatternCommandHandler(IWeeklyShiftPatternRepository patternRepo, IUnitOfWork unitOfWork)
        {
            _patternRepo = patternRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateWeeklyPatternCommand request, CancellationToken token)
        {
            // Include(Items) yapmalıyız çünkü update sırasında validation çalışacak
            var pattern = await _patternRepo.GetByIdWithItemsAsync(request.Id, token);
            if (pattern == null) throw new Exception("Şablon bulunamadı.");

            // Domain Entity içindeki Update metodu, alt itemların saatlerini kontrol eder.
            pattern.UpdateShiftDetails(request.ShiftStart, request.ShiftEnd, request.DefaultVehicleId);

            _patternRepo.Update(pattern);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
