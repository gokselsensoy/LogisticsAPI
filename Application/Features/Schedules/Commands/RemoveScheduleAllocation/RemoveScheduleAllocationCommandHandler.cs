using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Schedules.Commands.RemoveScheduleAllocation
{
    public class RemoveScheduleAllocationCommandHandler : IRequestHandler<RemoveScheduleAllocationCommand, Unit>
    {
        private readonly IDailyWorkScheduleRepository _dailyRepo;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveScheduleAllocationCommandHandler(IDailyWorkScheduleRepository dailyRepo, IUnitOfWork unitOfWork)
        {
            _dailyRepo = dailyRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(RemoveScheduleAllocationCommand request, CancellationToken token)
        {
            var schedule = await _dailyRepo.GetByIdWithAllocationsAsync(request.ScheduleId, token);
            if (schedule == null) throw new Exception("Vardiya bulunamadı.");

            schedule.RemoveAllocation(request.AllocationId);

            _dailyRepo.Update(schedule);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
