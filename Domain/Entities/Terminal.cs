using Domain.SeedWork;

namespace Domain.Entities
{
    // TERMİNAL (Depo / Çıkış Noktası)
    // Hem Supplier'ın hem Transporter'ın terminali olabilir.
    public class Terminal : Entity, IAggregateRoot
    {
        public string DepartmentId { get; private set; }
        public string Name { get; private set; }
        public Location Location { get; private set; }

        public Guid CompanyId { get; private set; }

        public Terminal(string name, Location location, Guid companyId)
        {
            Id = Guid.NewGuid();
            Name = name;
            Location = location;
            CompanyId = companyId;
        }
    }
}