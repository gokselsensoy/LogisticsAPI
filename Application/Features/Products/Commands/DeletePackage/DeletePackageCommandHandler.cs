using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Products.Commands.DeletePackage
{
    public class DeletePackageCommandHandler : IRequestHandler<DeletePackageCommand, Unit>
    {
        private readonly IProductRepository _productRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public DeletePackageCommandHandler(IProductRepository productRepo, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _productRepo = productRepo;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(DeletePackageCommand request, CancellationToken token)
        {
            var product = await _productRepo.GetByIdWithPackagesAsync(request.ProductId, token);
            if (product == null) throw new Exception("Ürün bulunamadı.");

            if (product.SupplierId != _currentUser.CompanyId)
                throw new UnauthorizedAccessException("Yetkisiz işlem.");

            // Domain metodunu çağır (Listeden çıkarır)
            // Ancak EF Core'da listeden çıkarmak Soft Delete yapmayabilir (Relation koparabilir).
            // En garantisi Package'ı bulup context'ten silmektir.

            var package = product.Packages.FirstOrDefault(x => x.Id == request.PackageId);
            if (package == null) throw new Exception("Paket bulunamadı.");

            // Eğer Package için repository yoksa UnitOfWork veya DbContext üzerinden erişim gerekebilir.
            // Ancak DDD'de child entityler Root üzerinden yönetilir.
            // Pratik Çözüm: Package entity'sine IsDeleted=true setlemek için bir metot ekleyip çağırmak.
            // Veya Generic Repository üzerinden Remove çağırmak (Eğer Package IEntity ise).

            // Yöntem 1: Repository üzerinden (Eğer IRepository<Package> varsa veya IRepository<object> ise)
            // _packageRepo.Remove(package); 

            // Yöntem 2: Domain üzerinden (FullAudited olduğu için)
            // Product içinde RemovePackage metodu listeden siler. EF Core bunu "Delete" olarak algılar.
            // Interceptor bunu yakalar ve "Soft Delete"e çevirir.
            product.RemovePackage(request.PackageId);

            _productRepo.Update(product);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
