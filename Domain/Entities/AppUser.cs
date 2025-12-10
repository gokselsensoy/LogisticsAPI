using Domain.Events;
using Domain.SeedWork;

namespace Domain.Entities
{
    public class AppUser : Entity, IAggregateRoot
    {
        public Guid IdentityId { get; private set; }
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public string PhoneNumber { get; private set; }

        private AppUser() { }

        public static AppUser Create(Guid identityId, string email, string phone, string fullName)
        {
            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                IdentityId = identityId,
                Email = email,
                PhoneNumber = phone,
                FullName = fullName
            };

            user.AddDomainEvent(new AppUserCreatedEvent(user.Id, email, fullName));

            return user;
        }

        public void SyncEmail(string newEmail)
        {
            if (!string.IsNullOrWhiteSpace(newEmail) && Email != newEmail)
            {
                Email = newEmail;
            }
        }
    }
}