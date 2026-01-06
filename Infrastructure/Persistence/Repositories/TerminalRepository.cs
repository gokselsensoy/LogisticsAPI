using Domain.Entities.Departments;
using Domain.Entities.Inventory;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class TerminalRepository : BaseRepository<Terminal>, ITerminalRepository
    {
        public TerminalRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Terminal>> GetByCompanyIdAsync(Guid companyId, CancellationToken token)
        {
            var query = from t in _context.Set<Terminal>()
                        join d in _context.Set<Department>() on t.DepartmentId equals d.Id
                        where d.CompanyId == companyId && !t.IsDeleted && !d.IsDeleted // Soft Delete kontrolü
                        select t;

            return await query.ToListAsync(token);
        }

        public async Task<Terminal?> GetByInventoryIdAsync(Guid inventoryId, CancellationToken token)
        {
            // Inventory -> Terminal ilişkisi üzerinden gidiyoruz
            // Varsayım: Inventory entity'sinde TerminalId var.
            return await _context.Set<Inventory>()
                .Where(i => i.Id == inventoryId)
                .Select(i => i.Terminal) // Navigation Property: Inventory.Terminal
                .Include(t => t.Address) // Terminalin adresi (Varsayım: Terminal'de Address ValueObject veya ilişkisi var)
                .FirstOrDefaultAsync(token);
        }
    }
}
