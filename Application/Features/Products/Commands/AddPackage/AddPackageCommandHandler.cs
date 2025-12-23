using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Products.Commands.AddPackage
{
    public class AddPackageCommandHandler : IRequestHandler<AddPackageCommand, Guid>
    {
        private readonly IProductRepository _productRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public AddPackageCommandHandler(IProductRepository productRepo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _productRepo = productRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(AddPackageCommand request, CancellationToken token)
        {
            // 1. Ürünü Getir
            var product = await _productRepo.GetByIdWithPackagesAsync(request.ProductId, token);
            if (product == null) throw new Exception("Ürün bulunamadı.");

            // 2. Sahiplik Kontrolü (Güvenlik)
            if (product.SupplierId != _currentUser.CompanyId) // Veya CompanyId
                throw new UnauthorizedAccessException("Bu ürüne paket ekleme yetkiniz yok.");

            // 3. Value Object'leri Oluştur
            var price = new Money(request.PriceAmount, request.Currency);
            var dimensions = new Dimensions(request.Width, request.Height, request.Length, request.Weight);
            Money? deposit = request.IsReturnable && request.DepositAmount.HasValue
                ? new Money(request.DepositAmount.Value, request.Currency)
                : null;

            // 4. Domain Metodunu Çağır
            product.AddPackage(
                request.Name,
                request.Type,
                request.ConversionFactor,
                price,
                dimensions,
                request.Barcode,
                request.IsReturnable,
                deposit
            );

            await _unitOfWork.SaveChangesAsync(token);

            // Eklenen son paketin ID'sini bulup dönebiliriz
            return product.Packages.Last().Id;
        }
    }
}
