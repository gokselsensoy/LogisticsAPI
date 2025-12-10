using Application.Abstractions.EntityRepositories;
using AutoMapper;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.QueryRepositories
{
    public class OrderQueryRepository : IOrderQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderQueryRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
