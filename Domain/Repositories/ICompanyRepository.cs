using Domain.Entities.Company;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<Company?> GetByIdWithDepartmentsAsync(Guid id, CancellationToken token);
    }
}
