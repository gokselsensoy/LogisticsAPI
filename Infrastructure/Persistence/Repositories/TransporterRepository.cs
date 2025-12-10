using Domain.Entities.Company;
using Domain.Repositories;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class TransporterRepository : BaseRepository<Transporter>, ITransporterRepository
    {
        public TransporterRepository(ApplicationDbContext context) : base(context)
        {
        }
    }

}
