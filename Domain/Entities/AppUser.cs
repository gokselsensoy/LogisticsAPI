using Domain.Events;
using Domain.SeedWork;

namespace Domain.Entities
{
    public class AppUser : FullAuditedEntity, IAggregateRoot
    {
        public Guid IdentityId { get; private set; }
        public string Email { get; private set; }

        private AppUser() { }

        public static AppUser Create(Guid identityId, string email)
        {
            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                IdentityId = identityId,
                Email = email,
                IsActive = true
            };

            user.AddDomainEvent(new AppUserCreatedEvent(user.Id, email));

            return user;
        }

        public void SyncEmail(string newEmail)
        {
            if (!string.IsNullOrWhiteSpace(newEmail) && Email != newEmail)
            {
                Email = newEmail;
            }
        }

        public void DeactivateAndScrambleEmail()
        {
            // Email'i bozuyoruz ki tekrar aynı email ile kayıt olunabilsin.
            Email = $"deleted_{Guid.NewGuid()}@deleted.com";
            IsActive = false;
            // İstersen IsDeleted=true da yapabilirsin ama email'i değiştirmek Unique Index için şart.
        }
    }
}