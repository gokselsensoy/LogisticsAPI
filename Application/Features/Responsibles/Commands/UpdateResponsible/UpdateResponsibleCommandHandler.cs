using Application.Abstractions.Services;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Responsibles.Commands.UpdateResponsible
{
    public class UpdateResponsibleCommandHandler : IRequestHandler<UpdateResponsibleCommand, Unit>
    {
        private readonly ICorporateResponsibleRepository _responsibleRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateResponsibleCommandHandler(ICorporateResponsibleRepository responsibleRepo, ICustomerRepository customerRepo, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _responsibleRepo = responsibleRepo;
            _customerRepo = customerRepo;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateResponsibleCommand request, CancellationToken token)
        {
            var currentResponsible = await _responsibleRepo.GetByAppUserIdAsync(_currentUser.AppUserId, token);
            if (currentResponsible == null || !currentResponsible.IsAdmin())
                throw new UnauthorizedAccessException("Yetkisiz işlem.");

            var targetResponsible = await _responsibleRepo.GetByIdAsync(request.ResponsibleId, token);
            if (targetResponsible == null) throw new Exception("Sorumlu bulunamadı.");

            // Kendi şirketinde mi?
            if (targetResponsible.CorporateCustomerId != currentResponsible.CorporateCustomerId)
                throw new UnauthorizedAccessException("Başka şirketin çalışanını düzenleyemezsiniz.");

            targetResponsible.UpdateDetails(request.FullName, request.Phone, request.Roles);

            _responsibleRepo.Update(targetResponsible);
            await _unitOfWork.SaveChangesAsync(token);

            return Unit.Value;
        }
    }
}
