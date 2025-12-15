using Domain.Entities.Customer;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class CorporateCustomerRepository : BaseRepository<CorporateCustomer>, ICorporateCustomerRepository
    {
        public CorporateCustomerRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
