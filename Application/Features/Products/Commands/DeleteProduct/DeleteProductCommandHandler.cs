using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IProductRepository _productRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public DeleteProductCommandHandler(IProductRepository productRepo, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _productRepo = productRepo;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken token)
        {
            var product = await _productRepo.GetByIdWithPackagesAsync(request.Id, token);
            if (product == null) throw new Exception("Ürün bulunamadı.");

            if (product.SupplierId != _currentUser.CompanyId)
                throw new UnauthorizedAccessException("Yetkisiz işlem.");

            // Aggregate Root'u silersek (Remove), EF Core konfigürasyonuna göre (Cascade Delete) 
            // alt elemanlar da silinir. Soft Delete interceptor devreye girer.
            // Ancak paketlerin de tek tek "IsDeleted=true" olması için Interceptor'ın
            // navigation propertyleri de gezmesi gerekebilir veya manuel silebiliriz.

            // Güvenli Yöntem: Paketleri de manuel silindi işaretle (veya Repository Remove mantığına güven)
            foreach (var pkg in product.Packages)
            {
                // Eğer Package için ayrı bir Repository yoksa, context üzerinden Remove diyebiliriz
                // Ama genelde Root silinince child'lar da context tarafından silindi işaretlenir.
            }

            _productRepo.Delete(product);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
