using Application.Abstractions.Services;
using Domain.Entities.Order;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Baskets.Commands.AddItemToBasket
{
    public class AddItemToBasketCommandHandler : IRequestHandler<AddItemToBasketCommand, Guid>
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IProductRepository _productRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public AddItemToBasketCommandHandler(
            IBasketRepository basketRepo,
            IProductRepository productRepo,
            ICurrentUserService currentUser,
            IUnitOfWork unitOfWork)
        {
            _basketRepo = basketRepo;
            _productRepo = productRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(AddItemToBasketCommand request, CancellationToken token)
        {
            // 1. Profil ID (Customer ID)
            Guid customerId = _currentUser.ProfileId ?? throw new UnauthorizedAccessException("Profil seçilmedi.");

            // 2. Paketi Bul (SupplierId lazım)
            // GetPackageByIdAsync içinde Product'ı Include etmiştik.
            var package = await _productRepo.GetPackageByIdAsync(request.PackageId, token);
            if (package == null) throw new Exception("Paket bulunamadı.");

            // 3. Kullanıcının sepeti var mı?
            var basket = await _basketRepo.GetByCustomerIdAsync(customerId, token);

            bool isNewBasket = false;

            if (basket == null)
            {
                // Yoksa oluştur
                basket = new Basket(customerId);
                _basketRepo.Add(basket);
                isNewBasket = true;
            }

            // 4. Domain Metodunu Çağır
            // package.Product.SupplierId kullanıyoruz
            basket.AddItem(request.PackageId, package.Product.SupplierId, request.Quantity);

            // 5. Kaydet (Eğer update ise _basketRepo.Update(basket) çağrılabilir base repo yapına göre)
            if (!isNewBasket)
            {
                _basketRepo.Update(basket);
            }

            await _unitOfWork.SaveChangesAsync(token);

            return basket.Id;
        }
    }
}
