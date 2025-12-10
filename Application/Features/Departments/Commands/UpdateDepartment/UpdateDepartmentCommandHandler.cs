using Application.Abstractions.Services;
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;
using NetTopologySuite.Geometries;

namespace Application.Features.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IWorkerRepository _workerRepo;
        private readonly ICompanyRepository _companyRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly GeometryFactory _geometryFactory;

        public UpdateDepartmentCommandHandler(
            ICurrentUserService currentUser,
            IWorkerRepository workerRepo,
            ICompanyRepository companyRepo,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _workerRepo = workerRepo;
            _companyRepo = companyRepo;
            _unitOfWork = unitOfWork;
            _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        }

        public async Task Handle(UpdateDepartmentCommand request, CancellationToken token)
        {
            // 1. Yetki Kontrolü
            var worker = await _workerRepo.GetByAppUserIdWithCompanyAsync(_currentUser.UserId, token);
            if (worker == null || (!worker.Roles.Contains(WorkerRole.Admin) && !worker.Roles.Contains(WorkerRole.Admin)))
                throw new UnauthorizedAccessException("Yetkiniz yok.");

            // 2. Şirketi ve Departmanları Çek
            // (Repo'da Include(d => d.Departments) olduğundan emin ol)
            var company = await _companyRepo.GetByIdWithDepartmentsAsync(worker.CompanyId, token);
            if (company == null) throw new Exception("Şirket bulunamadı.");

            // 3. Yeni Konum ve Adres Oluştur
            var location = _geometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));

            var newAddress = new Address(
                request.Street,
                request.BuildingNo,
                request.ZipCode,
                request.City,
                request.Country,
                location,
                request.Floor,
                request.Door
            );

            // 4. Domain Logic: Güncelle
            // Company içindeki "UpdateDepartment" metodumuz; 
            // - UpdateDetails
            // - Relocate
            // - AssignManager 
            // metodlarını sırasıyla çağırıp işi bitiriyor.
            company.UpdateDepartment(
                request.DepartmentId,
                request.Name,
                newAddress,
                request.Phone,
                request.Email,
                request.ManagerId
            );

            // 5. Kaydet
            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}
