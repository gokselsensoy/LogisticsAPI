using Domain.Entities.Company;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Company?> GetByIdWithDepartmentsAsync(Guid id, CancellationToken token)
        {
            return await _context.Set<Company>()
                                 .Include(c => c.Departments)
                                 .FirstOrDefaultAsync(c => c.Id == id, token);
        }
    }
}
