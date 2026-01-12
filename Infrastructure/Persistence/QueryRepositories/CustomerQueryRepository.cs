using Application.Abstractions.EntityRepositories;
using Application.Features.CustomerAddresses.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities.Customers;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.QueryRepositories
{
    public class CustomerQueryRepository : ICustomerQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CustomerQueryRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<AddressSelectionDto>> GetAddressesForIndividualAsync(Guid customerId, CancellationToken token)
        {
            return await _context.Set<CustomerAddress>()
                .AsNoTracking()
                .Where(a => a.CustomerId == customerId && !a.IsDeleted)
                .ProjectTo<AddressSelectionDto>(_mapper.ConfigurationProvider)
                .ToListAsync(token);
        }

        // 2. KURUMSAL SORUMLU İÇİN (Admin Kontrollü)
        public async Task<List<AddressSelectionDto>> GetAddressesForResponsibleAsync(Guid responsibleId, CancellationToken token)
        {
            // A. Önce Sorumlunun Yetkisini ve Şirketini Bulalım
            // IsAdmin kolon olmadığı için, Roles listesinde "Admin" var mı diye bakıyoruz.
            var responsibleInfo = await _context.Set<CorporateResponsible>()
                .AsNoTracking()
                .Where(r => r.Id == responsibleId)
                .Select(r => new
                {
                    // DİKKAT: 'Name' alanı CorporateRole içindeki rol adıdır.
                    // Sende Enum ise: r.Roles.Any(x => x.RoleType == RoleType.Admin)
                    // Sende String ise: r.Roles.Any(x => x.Name == "Admin")
                    IsAdmin = r.Roles.Any(role => role == CorporateRole.Admin),

                    r.CorporateCustomerId
                })
                .FirstOrDefaultAsync(token);

            if (responsibleInfo == null) return new List<AddressSelectionDto>();

            // B. Duruma Göre Sorgu
            if (responsibleInfo.IsAdmin)
            {
                // SENARYO 1: ADMİN (Rollerinde Admin var)
                // Şirketin (CorporateCustomer) TÜM adreslerini çeker.
                return await _context.Set<CustomerAddress>()
                    .AsNoTracking()
                    .Where(a => a.CustomerId == responsibleInfo.CorporateCustomerId && !a.IsDeleted)
                    .ProjectTo<AddressSelectionDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(token);
            }
            else
            {
                // SENARYO 2: STANDART SORUMLU
                // Sadece Map tablosunda eşleştirilmiş adresleri çeker.
                return await _context.Set<CorporateAddressResponsibleMap>()
                    .AsNoTracking()
                    .Where(map => map.ResponsibleId == responsibleId)
                    .ProjectTo<AddressSelectionDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(token);
            }
        }
    }
}
