using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.CustomerAddresses.Commands.AssignAddressToResponsible
{
    public class AssignAddressToResponsibleCommandHandler : IRequestHandler<AssignAddressToResponsibleCommand, Unit>
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly ICorporateResponsibleRepository _responsibleRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public AssignAddressToResponsibleCommandHandler(ICustomerRepository customerRepo, ICorporateResponsibleRepository responsibleRepo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _customerRepo = customerRepo;
            _responsibleRepo = responsibleRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AssignAddressToResponsibleCommand request, CancellationToken token)
        {
            // 1. İşlemi Yapan (Admin) Kontrolü
            var admin = await _responsibleRepo.GetByAppUserIdAsync(_currentUser.UserId, token);
            if (admin == null || !admin.IsAdmin())
                throw new UnauthorizedAccessException("Atama yapma yetkiniz yok.");

            // 2. Hedef Çalışan Kontrolü
            var targetResponsible = await _responsibleRepo.GetByIdWithAssignmentsAsync(request.TargetResponsibleId, token);
            if (targetResponsible == null) throw new Exception("Hedef çalışan bulunamadı.");

            // Aynı şirketteler mi?
            if (admin.CorporateCustomerId != targetResponsible.CorporateCustomerId)
                throw new UnauthorizedAccessException("Farklı şirketin çalışanına atama yapamazsınız.");

            // 3. Adres Kontrolü (Adres gerçekten bu şirkete mi ait?)
            var company = await _customerRepo.GetByIdWithAddressesAsync(admin.CorporateCustomerId, token);
            if (!company.Addresses.Any(a => a.Id == request.AddressId))
                throw new Exception("Adres şirketinize ait değil.");

            // 4. Atama Yap
            targetResponsible.AssignAddress(request.AddressId);

            _responsibleRepo.Update(targetResponsible);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
