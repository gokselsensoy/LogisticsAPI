using Domain.Entities.Company;
using Domain.Repositories;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
