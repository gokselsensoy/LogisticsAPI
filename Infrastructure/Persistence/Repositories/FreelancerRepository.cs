using Domain.Entities.Company;
using Domain.Repositories;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class FreelancerRepository : BaseRepository<Freelancer>, IFreelancerRepository
    {
        public FreelancerRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
