using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Entities.Company;
using Domain.Entities.Departments;
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Auth.Commands.RegisterSupplier
{
    public class RegisterSupplierCommandHandler : IRequestHandler<RegisterSupplierCommand, Guid>
    {
        private readonly IIdentityService _identityService;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterSupplierCommandHandler(
            IIdentityService identityService,
            ISupplierRepository supplierRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _supplierRepository = supplierRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterSupplierCommand request, CancellationToken token)
        {
            // 1. IdentityAPI'de Kullanıcı Oluştur (S2S)
            var identityId = await _identityService.CreateUserAsync(
                request.Email, request.Password, "Supplier", token);

            if (identityId == null) throw new Exception("Kullanıcı oluşturulamadı.");

            var localUser = AppUser.Create(identityId.Value, request.Email, request.Phone, request.FullName);
            _userRepository.Add(localUser);

            var supplier = new Supplier(request.CompanyName, request.CvrNumber, request.Email);

            var mainDept = supplier.CreateDefaultDepartment();

            var ownerWorker = new Worker(
                supplier.Id,
                mainDept.Id,
                localUser.Id,
                new List<WorkerRole> { WorkerRole.Admin }
            );

            supplier.AddWorker(ownerWorker);

            _supplierRepository.Add(supplier);
            await _unitOfWork.SaveChangesAsync(token);

            return supplier.Id;
        }
    }
}
