using Domain.SeedWork;

namespace Domain.Entities
{
    // =========================================================================
    // 2. IDENTITY & ACTORS (KİMLİK VE AKTÖRLER)
    // =========================================================================

    // [AppUser]: Sisteme giren GERÇEK KİŞİ (Ahmet, Mehmet)
    // Bu tabloda şirket bilgisi YOKTUR. Sadece kişi bilgisi vardır.
    public class AppUser : Entity, IAggregateRoot
    {
        public Guid IdentityId { get; private set; }
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public string PhoneNumber { get; private set; }

        public AppUser(Guid identityId, string email, string fullName)
        {
            Id = Guid.NewGuid();
            IdentityId = identityId;
            Email = email;
            FullName = fullName;
        }
    }
}