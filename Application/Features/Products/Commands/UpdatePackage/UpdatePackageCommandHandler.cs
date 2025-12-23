using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Products.Commands.UpdatePackage
{
    public class UpdatePackageCommandHandler : IRequestHandler<UpdatePackageCommand, Unit>
    {
        private readonly IProductRepository _productRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePackageCommandHandler(IProductRepository productRepo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _productRepo = productRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdatePackageCommand request, CancellationToken token)
        {
            // 1. Aggregate Root'u (Product) ve alt paketlerini getir
            var product = await _productRepo.GetByIdWithPackagesAsync(request.ProductId, token);
            if (product == null) throw new Exception("Ürün bulunamadı.");

            // 2. Yetki Kontrolü
            if (product.SupplierId != _currentUser.CompanyId)
                throw new UnauthorizedAccessException("Bu paketi güncelleme yetkiniz yok.");

            // 3. Value Object Oluşturma
            var price = new Money(request.PriceAmount, request.Currency);
            var dimensions = new Dimensions(request.Width, request.Height, request.Length, request.Weight);

            Money? deposit = null;
            if (request.IsReturnable && request.DepositAmount.HasValue)
            {
                deposit = new Money(request.DepositAmount.Value, request.Currency);
            }

            // 4. Domain Metodu Çağrısı
            product.UpdatePackage(
                request.PackageId,
                request.Name,
                price,
                dimensions,
                request.IsReturnable,
                deposit
            );

            // 5. Kaydet
            _productRepo.Update(product);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
