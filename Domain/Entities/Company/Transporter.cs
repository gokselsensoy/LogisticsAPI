namespace Domain.Entities.Company
{
    public class Transporter : Company
    {

        public Transporter(string name, string? cvrNumber)
            : base(name, cvrNumber)
        {
        }
    }
}