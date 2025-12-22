using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Schedules.Commands.AddPatternItem
{
    public class AddPatternItemCommandHandler : IRequestHandler<AddPatternItemCommand, Unit>
    {
        private readonly IWeeklyShiftPatternRepository _patternRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddPatternItemCommandHandler(IWeeklyShiftPatternRepository patternRepo, IUnitOfWork unitOfWork)
        {
            _patternRepo = patternRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddPatternItemCommand request, CancellationToken token)
        {
            var pattern = await _patternRepo.GetByIdWithItemsAsync(request.PatternId, token);
            if (pattern == null) throw new Exception("Şablon bulunamadı.");

            // Domain mantığı (Overlap kontrolü vs.) Entity içinde çalışır
            pattern.AddPatternItem(request.StartTime, request.EndTime, request.Type, request.DefaultVehicleId);

            _patternRepo.Update(pattern);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
