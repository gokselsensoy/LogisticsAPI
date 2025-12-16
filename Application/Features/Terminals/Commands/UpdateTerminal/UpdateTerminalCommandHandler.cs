using Application.Abstractions.Services;
using Domain.Entities.Departments;
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;
using NetTopologySuite.Geometries;

namespace Application.Features.Terminals.Commands.UpdateTerminal
{
    public class UpdateTerminalCommandHandler : IRequestHandler<UpdateTerminalCommand>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IWorkerRepository _workerRepo;
        private readonly ITerminalRepository _terminalRepo;
        private readonly IDepartmentRepository _departmentRepo; // Departman kontrolü için
        private readonly IUnitOfWork _unitOfWork;
        private readonly GeometryFactory _geometryFactory;

        public UpdateTerminalCommandHandler(
            ICurrentUserService currentUser,
            IWorkerRepository workerRepo,
            ITerminalRepository terminalRepo,
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _workerRepo = workerRepo;
            _terminalRepo = terminalRepo;
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
            _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        }

        public async Task Handle(UpdateTerminalCommand request, CancellationToken token)
        {
            // 1. Yetki
            if (!_currentUser.CompanyId.HasValue)
                throw new UnauthorizedAccessException("Bu işlem için bir şirket profili ile giriş yapmalısınız.");

            if (!_currentUser.Roles.Contains("Admin")) // Rol string olarak geliyorsa
                throw new UnauthorizedAccessException("Departman ekleme yetkiniz yok.");

            // 2. Terminali Bul
            var terminal = await _terminalRepo.GetByIdAsync(request.TerminalId, token);
            if (terminal == null) throw new Exception("Terminal bulunamadı.");

            // 3. Şirket Kontrolü (Zincirleme)
            // Terminal -> Department -> Company
            // Bu kontrolü yapmak için Terminali çekerken Include(t => t.Department) yapman lazım
            // Veya basitçe departmanı çekip kontrol edebilirsin.
            var department = await _departmentRepo.GetByIdAsync(terminal.DepartmentId, token);
            if (department.CompanyId != _currentUser.CompanyId.Value)
                throw new UnauthorizedAccessException("Bu terminal sizin şirketinize ait değil.");

            // 4. Update İşlemleri
            var location = _geometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));
            var newAddress = new Address(request.Street, request.BuildingNo, request.ZipCode, request.City, request.Country, location, request.Floor, request.Door);

            terminal.UpdateDetails(request.Name, request.Phone, request.Email);
            terminal.Relocate(newAddress);
            terminal.AssignManager(request.ManagerId);

            // 5. Kaydet
            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}
