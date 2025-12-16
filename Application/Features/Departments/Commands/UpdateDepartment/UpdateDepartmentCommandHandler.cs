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
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly GeometryFactory _geometryFactory;

        public UpdateDepartmentCommandHandler(
            ICurrentUserService currentUser,
            IDepartmentRepository departmentRepo,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _departmentRepo = departmentRepo;
            _unitOfWork = unitOfWork;
            _geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        }

        public async Task Handle(UpdateDepartmentCommand request, CancellationToken token)
        {
            // 1. Yetki Kontrolü
            if (!_currentUser.CompanyId.HasValue)
                throw new UnauthorizedAccessException("Bu işlem için bir şirket profili ile giriş yapmalısınız.");

            if (!_currentUser.Roles.Contains("Admin")) // Rol string olarak geliyorsa
                throw new UnauthorizedAccessException("Departman güncelleme yetkiniz yok.");

            // 2. Şirketi ve Departmanları Çek
            // (Repo'da Include(d => d.Departments) olduğundan emin ol)
            var department = await _departmentRepo.GetByIdAsync(request.DepartmentId, token);
            if (department == null) throw new Exception("Departman bulunamadı.");

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

            department.UpdateDetails(request.Name, request.Phone, request.Email);
            department.Relocate(newAddress);
            department.AssignManager(request.ManagerId);

            _departmentRepo.Update(department);

            // 5. Kaydet
            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}
