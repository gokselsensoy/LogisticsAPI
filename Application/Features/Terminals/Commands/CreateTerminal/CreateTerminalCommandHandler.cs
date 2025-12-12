using Application.Abstractions.Services;
using Domain.Entities.Departments;
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;
using NetTopologySuite.Geometries;

namespace Application.Features.Terminals.Commands.CreateTerminal
{
    public class CreateTerminalCommandHandler : IRequestHandler<CreateTerminalCommand, Guid>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IWorkerRepository _workerRepo;
        private readonly ITerminalRepository _terminalRepo;
        private readonly IDepartmentRepository _departmentRepo; // Departman kontrolü için
        private readonly IUnitOfWork _unitOfWork;
        private readonly GeometryFactory _geometryFactory;

        public CreateTerminalCommandHandler(
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

        public async Task<Guid> Handle(CreateTerminalCommand request, CancellationToken token)
        {
            // 1. İşlemi yapanı bul
            var worker = await _workerRepo.GetByAppUserIdWithCompanyAsync(_currentUser.UserId, token);
            if (worker == null || (!worker.Roles.Contains(WorkerRole.Admin)))
                throw new UnauthorizedAccessException("Yetkiniz yok.");

            // 2. GÜVENLİK KONTROLÜ: Departman bu şirkete mi ait?
            // Kullanıcı kafasına göre ID gönderip başka şirketin departmanına terminal ekleyemesin.
            var department = await _departmentRepo.GetByIdAsync(request.DepartmentId, token);
            if (department == null) throw new Exception("Departman bulunamadı.");

            if (department.CompanyId != worker.CompanyId)
                throw new UnauthorizedAccessException("Bu departman sizin şirketinize ait değil.");

            // 3. Adres ve Konum
            var location = _geometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));
            var address = new Address(request.Street, request.BuildingNo, request.ZipCode, request.City, request.Country, location, request.Floor, request.Door);

            // 4. Terminali Oluştur
            var terminal = new Terminal(
                request.DepartmentId,
                request.Name,
                address,
                request.Phone,
                request.Email,
                request.ManagerId
            );

            // 5. Kaydet
            _terminalRepo.Add(terminal);
            await _unitOfWork.SaveChangesAsync(token);

            return terminal.Id;
        }
    }
}
