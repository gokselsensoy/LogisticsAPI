using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IProductRepository _productRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(IProductRepository productRepo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _productRepo = productRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken token)
        {
            // 1. Ürünü Getir
            var product = await _productRepo.GetByIdAsync(request.Id, token);
            if (product == null) throw new Exception("Ürün bulunamadı.");

            // 2. Güvenlik: Sadece ürünü oluşturan Supplier güncelleyebilir
            if (product.SupplierId != _currentUser.CompanyId)
                throw new UnauthorizedAccessException("Bu ürünü güncelleme yetkiniz yok.");

            // 3. Güncelle
            product.UpdateDetails(request.Name, request.Description);

            // 4. Kaydet
            _productRepo.Update(product);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
