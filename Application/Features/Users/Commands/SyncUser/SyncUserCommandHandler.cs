using Application.Abstractions.Repositories;
using Domain.Entities;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Users.Commands.SyncUser
{
    public class SyncUserCommandHandler : IRequestHandler<SyncUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SyncUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(SyncUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByIdentityIdAsync(request.IdentityId, cancellationToken);

            if (existingUser == null)
            {
                //Log
                return;
            }

            existingUser.SyncEmail(request.Email);

            _userRepository.Update(existingUser);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
