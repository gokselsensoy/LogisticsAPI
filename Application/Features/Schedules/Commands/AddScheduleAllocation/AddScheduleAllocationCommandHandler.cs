using Domain.Repositories;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Schedules.Commands.AddScheduleAllocation
{
    public class AddScheduleAllocationCommandHandler : IRequestHandler<AddScheduleAllocationCommand, Unit>
    {
        private readonly IDailyWorkScheduleRepository _dailyRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddScheduleAllocationCommandHandler(IDailyWorkScheduleRepository dailyRepo, IUnitOfWork unitOfWork)
        {
            _dailyRepo = dailyRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AddScheduleAllocationCommand request, CancellationToken token)
        {
            // Include(Allocations) şart! Çakışma kontrolü için.
            var schedule = await _dailyRepo.GetByIdWithAllocationsAsync(request.ScheduleId, token);
            if (schedule == null) throw new Exception("Vardiya bulunamadı.");

            // Vehicle Müsaitlik Kontrolü (Repository'ye eklediğimiz metod)
            if (request.VehicleId.HasValue)
            {
                var isVehicleAvailable = await _dailyRepo.IsVehicleAvailableAsync(request.VehicleId.Value, request.StartTime, request.EndTime, token);
                if (!isVehicleAvailable)
                    throw new Exception("Seçilen araç belirtilen saat aralığında başka bir görevde.");
            }

            // Domain Logic (Personel Çakışma Kontrolü burada yapılır)
            schedule.AddAllocation(
                new TimeRange(request.StartTime, request.EndTime),
                request.Type,
                request.VehicleId
            );

            _dailyRepo.Update(schedule);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
