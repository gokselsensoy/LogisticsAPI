using Application.Abstractions.EntityRepositories;
using Application.Abstractions.Services;
using Application.Features.Baskets.DTOs;
using MediatR;

namespace Application.Features.Baskets.Queries.GetMyBasket
{
    public class GetMyBasketQuery : IRequest<BasketDto> 
    {
    }

    public class GetMyBasketQueryHandler : IRequestHandler<GetMyBasketQuery, BasketDto>
    {
        private readonly IBasketQueryRepository _basketQueryRepo;
        private readonly ICurrentUserService _currentUser;

        public GetMyBasketQueryHandler(IBasketQueryRepository basketQueryRepo, ICurrentUserService currentUser)
        {
            _basketQueryRepo = basketQueryRepo;
            _currentUser = currentUser;
        }

        public async Task<BasketDto> Handle(GetMyBasketQuery request, CancellationToken token)
        {
            // 1. Kullanıcı ID'sini al
            // (ProfileId yoksa UserId'ye göre fallback mantığı kurabilirsin ama genelde ProfileId ile çalışırız)
            Guid? customerId;
            if (_currentUser.ProfileType == "CorporateResponsible")
            {
                customerId = _currentUser.CompanyId;
            } 
            else if (_currentUser.ProfileType == "IndividualCustomer")
            {
                customerId = _currentUser.ProfileId;
            }
            else
            {
                throw new UnauthorizedAccessException("Sepeti görüntülemek için bir profil seçmelisiniz.");
            }

            // 2. Query Repository'e git
            var basketDto = await _basketQueryRepo.GetBasketDetailsByCustomerIdAsync(customerId.Value, token);

            // 3. Eğer sepet henüz oluşmamışsa (null), boş bir model dön (Front-end hataya düşmesin)
            if (basketDto == null)
            {
                return new BasketDto
                {
                    Items = new List<BasketItemDto>(),
                    TotalPrice = 0
                };
            }

            return basketDto;
        }
    }
}
