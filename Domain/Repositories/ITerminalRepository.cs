using Domain.Entities.Departments;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface ITerminalRepository : IRepository<Terminal>
    {
        Task<List<Terminal>> GetByCompanyIdAsync(Guid companyId, CancellationToken token);
        Task<Terminal?> GetByInventoryIdAsync(Guid inventoryId, CancellationToken token);
    }
    
}
