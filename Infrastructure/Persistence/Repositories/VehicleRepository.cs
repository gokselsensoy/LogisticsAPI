using Domain.Entities.Departments;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class VehicleRepository : BaseRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Vehicle?> GetByIdActiveAsync(Guid id, CancellationToken token)
        {
            return await _context.Set<Vehicle>()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, token);
        }

        // Plaka kontrolü (Sadece aktif araçlarda ara)
        // Eğer bir araç silindiyse (Soft Deleted), o plaka tekrar kullanılabilir olmalı mantığıyla hareket ediyoruz.
        public async Task<bool> IsPlateExistsAsync(string plateNumber, CancellationToken token)
        {
            return await _context.Set<Vehicle>()
                .AnyAsync(x => x.PlateNumber == plateNumber && !x.IsDeleted, token);
        }
    }
}
