using Domain.SeedWork;

namespace Domain.Entities
{
    public class Department : Entity
    {
        public Guid CompanyId { get; private set; }
        public string Name { get; private set; }
        public Location Location { get; private set; }
    }
}