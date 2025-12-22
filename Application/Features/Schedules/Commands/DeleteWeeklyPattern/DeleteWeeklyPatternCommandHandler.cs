using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Schedules.Commands.DeleteWeeklyPattern
{
    public class DeleteWeeklyPatternCommandHandler : IRequestHandler<DeleteWeeklyPatternCommand, Unit>
    {
        private readonly IWeeklyShiftPatternRepository _patternRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteWeeklyPatternCommandHandler(IWeeklyShiftPatternRepository patternRepo, IUnitOfWork unitOfWork)
        {
            _patternRepo = patternRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteWeeklyPatternCommand request, CancellationToken token)
        {
            // 1. Şablonu Bul
            var pattern = await _patternRepo.GetByIdAsync(request.Id, token);

            if (pattern == null)
                throw new Exception("Haftalık vardiya şablonu bulunamadı.");

            // 2. Sil (Repository'deki Remove metodu çağrılır)
            // Interceptor sayesinde bu işlem Soft Delete'e dönüşecek.
            _patternRepo.Delete(pattern);

            // 3. Kaydet
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
