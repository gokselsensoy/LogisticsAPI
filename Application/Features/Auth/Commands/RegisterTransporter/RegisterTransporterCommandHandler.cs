using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities;
using Domain.Entities.Company;
using Domain.Entities.Departments;
using Domain.Enums;
using Domain.Repositories;
using Domain.SeedWork;
using MediatR;

namespace Application.Features.Auth.Commands.RegisterTransporter
{
    public class RegisterTransporterCommandHandler : IRequestHandler<RegisterTransporterCommand, Guid>
    {
        private readonly IIdentityService _identityService;
        private readonly ITransporterRepository _transporterRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterTransporterCommandHandler(
            IIdentityService identityService,
            ITransporterRepository transporterRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _transporterRepository = transporterRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterTransporterCommand request, CancellationToken token)
        {
            // 1. IdentityAPI'de Kullanıcı Oluştur (S2S)
            var identityId = await _identityService.CreateUserAsync(
                request.Email, request.Password, "Transporter", token);

            if (identityId == null) throw new Exception("Kullanıcı oluşturulamadı.");

            // 2. LogisticsAPI'de User Projeksiyonunu Oluştur (Senkronizasyon)
            var localUser = AppUser.Create(identityId.Value, request.Email, request.Phone, request.FullName);
            _userRepository.Add(localUser);

            // 3. Şirketi (Transporter) Oluştur
            var transporter = new Transporter(request.CompanyName, request.CvrNumber, request.Email);

            // 4. *** KRİTİK ADIM ***: Varsayılan Departmanı Oluştur
            var mainDept = transporter.CreateDefaultDepartment();
            // (Transporter constructor'ında veya burada _departments listesine eklenmeli)

            // 5. Worker (Admin/Owner) Oluştur ve Bağla
            var ownerWorker = new Worker(
                transporter.Id,
                mainDept.Id, // Oluşan departman ID'sini buraya veriyoruz!
                localUser.Id, // Bizim yerel User ID'miz (AppUserId)
                new List<WorkerRole> { WorkerRole.Admin }
            );

            transporter.AddWorker(ownerWorker); // Aggregate Root üzerinden ekle

            // 6. Kaydet
            _transporterRepository.Add(transporter);
            await _unitOfWork.SaveChangesAsync(token);

            return transporter.Id;
        }
    }
}
