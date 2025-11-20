using Domain.SeedWork;
using System.Drawing;

namespace Domain.Entities
{
    public class User : Entity
    {
        public Guid IdentityId { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }

        private User() { }

        public static User Create(Guid identityId, string email, string phoneNumber)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                IdentityId = identityId,
                Email = email,
                PhoneNumber = phoneNumber
            };
        }

        public void Update(string email)
        {
            Email = email;
        }
    }

    public class Company : Entity
    {
        public string Name { get; set; }
        public string CvrNo { get; set; }
        public CompanyType CompanyType { get; set; }
    }

    public enum CompanyType
    {
        Transporter,
        Supplier,
        RetailerCustomer,
        IndividualCustomer,
    }

    public class CustomerAddress : Entity
    {
        public Guid CompanyId { get; private set; }
        public Guid ResponsibleUserId { get; private set; }
        public Point Location { get; private set; }
        public string Address { get; private set; }
    }

    public class Worker : Entity
    {
        public Guid UserId { get; private set; }
        public Guid CompanyId { get; private set; }
        public string Email { get; private set; }
    }

    public class WorkerRoleMap : Entity
    {
        public Guid WorkerId { get; private set; }
        public Guid RoleId { get; private set; }
    }

    public class Role : Entity
    {
        public string Name { get; private set; }
    }
}
