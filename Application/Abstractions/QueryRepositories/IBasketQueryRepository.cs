using Application.Features.Baskets.DTOs;

namespace Application.Abstractions.EntityRepositories
{
    public interface IBasketQueryRepository
    {
        Task<BasketDto?> GetBasketDetailsByCustomerIdAsync(Guid customerId, CancellationToken token);
    }


}
