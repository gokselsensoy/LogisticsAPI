namespace Domain.Entities.Company
{
    public class Transporter : Company
    {
        private Transporter() : base(null!, null) { }

        public Transporter(string name, string? cvrNumber) : base(name, cvrNumber) { }
    }
}