using Application.Abstractions.Services;
using Domain.Entities.Inventory;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IProductRepository productRepo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _productRepo = productRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken token)
        {
            // 1. Supplier Kim? (Token'dan ProfileId alıyoruz. Bu Worker değil, Supplier/Company kullanıcısı olmalı)
            // Eğer sistemde SupplierId -> ProfileId eşleşmesi varsa:
            if (!_currentUser.CompanyId.HasValue)
                throw new UnauthorizedAccessException("Tedarikçi bilgisi bulunamadı.");

            // VEYA: CompanyId üzerinden de gidebiliriz. Senaryona göre değişir.
            // Guid supplierId = _currentUser.CompanyId ?? throw new ...
            Guid supplierId = _currentUser.CompanyId.Value; // Varsayım: ProfileId = SupplierId

            // 2. Ürünü Oluştur
            var product = Product.Create(supplierId, request.Name, request.Description, request.UnitType);

            // 3. Kaydet
            _productRepo.Add(product);
            await _unitOfWork.SaveChangesAsync(token);

            return product.Id;
        }
    }
}
