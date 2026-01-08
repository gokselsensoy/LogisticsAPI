using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Baskets.Commands.RemoveItemFromBasket
{
    public class RemoveItemFromBasketCommandHandler : IRequestHandler<RemoveItemFromBasketCommand, Unit>
    {
        private readonly IBasketRepository _basketRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveItemFromBasketCommandHandler(IBasketRepository basketRepo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _basketRepo = basketRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(RemoveItemFromBasketCommand request, CancellationToken token)
        {
            Guid customerId = _currentUser.ProfileId ?? throw new UnauthorizedAccessException("Profil seçilmedi.");

            var basket = await _basketRepo.GetByCustomerIdAsync(customerId, token);
            if (basket == null) throw new Exception("Sepet bulunamadı.");

            basket.RemoveItem(request.PackageId); // Tamamen siler

            _basketRepo.Update(basket);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
