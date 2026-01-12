using Domain.Entities.Companies;
using Domain.Entities.Departments;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface IWorkerRepository : IRepository<Worker>
    {
        Task<Worker?> GetByAppUserIdWithCompanyAsync(Guid appUserId, CancellationToken token);
        Task<List<(Worker Worker, Company Company)>> GetAllByAppUserIdWithCompanyAsync(Guid appUserId, CancellationToken token);
    }
}
