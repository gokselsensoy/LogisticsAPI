using Domain.SeedWork;

namespace Domain.Entities
{
    public class User : Entity
    {
        public Guid IdentityId { get; private set; }
        public string Email { get; private set; }

        private User() { }

        public static User Create(Guid identityId, string email)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                IdentityId = identityId,
                Email = email
            };
        }

        public void Update(string email)
        {
            Email = email;
        }
    }
}
