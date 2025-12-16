using Application.Abstractions.Services;
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using Domain.ValueObjects;
using MediatR;
using NetTopologySuite.Geometries;

namespace Application.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ICompanyRepository _companyRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly GeometryFactory _geometryFactory; // Konum oluşturucu

        public CreateDepartmentCommandHandler(
            ICurrentUserService currentUser,
            IWorkerRepository workerRepo,
            ICompanyRepository companyRepo,
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _companyRepo = companyRepo;
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
            // SRID 4326 (GPS/Dünya Standardı) için factory
            _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        }

        public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken token)
        {
            // 1. Yetki ve Şirket Kontrolü
            if (!_currentUser.CompanyId.HasValue)
                throw new UnauthorizedAccessException("Bu işlem için bir şirket profili ile giriş yapmalısınız.");

            if (!_currentUser.Roles.Contains("Admin")) // Rol string olarak geliyorsa
                throw new UnauthorizedAccessException("Departman ekleme yetkiniz yok.");

            var company = await _companyRepo.GetByIdAsync(_currentUser.CompanyId.Value, token);
            if (company == null) throw new Exception("Şirket bulunamadı.");

            // 2. Coğrafi Noktayı Oluştur (Lon, Lat sırasıyla!)
            var location = _geometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));

            // 3. Address Value Object Oluştur
            // (Floor ve Door burada otomatik parse edilecek)
            var address = new Address(
                request.Street,
                request.BuildingNo,
                request.ZipCode,
                request.City,
                request.Country,
                location,
                request.Floor,  // "st", "1" vb.
                request.Door
            );

            // 4. Domain Metodunu Çağır (Aggregate Root Üzerinden)
            var department = company.CreateDepartment(
                request.Name,
                address,
                request.Phone,
                request.Email,
                request.ManagerId
            );

            _departmentRepo.Add(department);

            // 5. Kaydet
            await _unitOfWork.SaveChangesAsync(token);

            // Son eklenen ID'yi döndür
            return department.Id;
        }
    }
}
