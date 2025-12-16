using Application.Abstractions.Services;
using Domain.Entities.Departments;
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;
using NetTopologySuite.Geometries;

namespace Application.Features.Terminals.Commands.DeleteTerminal
{
    public class DeleteTerminalCommandHandler : IRequestHandler<DeleteTerminalCommand>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ITerminalRepository _terminalRepo;
        private readonly IDepartmentRepository _departmentRepo; // Departman kontrolü için
        private readonly IUnitOfWork _unitOfWork;
        private readonly GeometryFactory _geometryFactory;

        public DeleteTerminalCommandHandler(
            ICurrentUserService currentUser,
            ITerminalRepository terminalRepo,
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _terminalRepo = terminalRepo;
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
            _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        }

        public async Task Handle(DeleteTerminalCommand request, CancellationToken token)
        {
            if (!_currentUser.CompanyId.HasValue)
                throw new UnauthorizedAccessException("Bu işlem için bir şirket profili ile giriş yapmalısınız.");

            if (!_currentUser.Roles.Contains("Admin")) // Rol string olarak geliyorsa
                throw new UnauthorizedAccessException("Departman ekleme yetkiniz yok.");

            var terminal = await _terminalRepo.GetByIdAsync(request.TerminalId, token);
            if (terminal == null) throw new Exception("Terminal bulunamadı.");

            // Şirket Kontrolü
            var department = await _departmentRepo.GetByIdAsync(terminal.DepartmentId, token);
            if (department.CompanyId != _currentUser.CompanyId.Value)
                throw new UnauthorizedAccessException();

            // Soft Delete (Interceptor araya girip IsDeleted=true yapacak)
            _terminalRepo.Delete(terminal); // Repository'de Delete metodu _dbSet.Remove(entity) çağırır

            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}
