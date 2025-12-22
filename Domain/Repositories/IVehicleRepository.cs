using Domain.Entities.Departments;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<Vehicle?> GetByIdActiveAsync(Guid id, CancellationToken token);
        Task<bool> IsPlateExistsAsync(string plateNumber, CancellationToken token);
    }
}
