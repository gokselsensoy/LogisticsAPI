using Domain.Entities.WorkSchedule;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface IDailyWorkScheduleRepository : IRepository<DailyWorkSchedule>
    {
        Task<DailyWorkSchedule?> GetByIdWithAllocationsAsync(Guid id, CancellationToken token);
        // O gün için bu çalışanın zaten kaydı var mı?
        Task<bool> ExistsAsync(Guid workerId, DateOnly date, CancellationToken token);

        // Görev ataması yaparken (Shipment için) müsaitlik kontrolü sorgusu
        Task<bool> IsWorkerAvailableAsync(Guid workerId, DateTime start, DateTime end, CancellationToken token);

        // Araç müsaitlik kontrolü
        Task<bool> IsVehicleAvailableAsync(Guid vehicleId, DateTime start, DateTime end, CancellationToken token);
    }
}
