using Domain.Entities.Companies;
using Domain.SeedWork;

namespace Domain.Repositories
{
    public interface IFreelancerRepository : IRepository<Freelancer>
    {
        Task<Freelancer?> GetByAppUserIdAsync(Guid appUserId, CancellationToken token);
    }
}
