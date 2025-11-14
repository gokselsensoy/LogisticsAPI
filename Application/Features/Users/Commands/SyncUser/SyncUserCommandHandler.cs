using Application.Abstractions.Repositories;
using Domain.Entities;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Users.Commands.SyncUser
{
    public class SyncUserCommandHandler : IRequestHandler<SyncUserCommand>
    {
        // === DEĞİŞİKLİK ===
        // DbContext'i IUserRepository ile değiştirdik
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SyncUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        // ===================

        public async Task Handle(SyncUserCommand request, CancellationToken cancellationToken)
        {
            // Artık arayüzümüz üzerinden konuşuyoruz
            var existingUser = await _userRepository.GetByIdentityIdAsync(request.IdentityId, cancellationToken);

            if (existingUser == null)
            {
                var newUser = User.Create(request.IdentityId, request.Email);
                _userRepository.Add(newUser);
            }
            else
            {
                existingUser.Update(request.Email);
                _userRepository.Update(existingUser);
            }

            // Değişiklikleri (Add/Update) DB'ye commit et
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
