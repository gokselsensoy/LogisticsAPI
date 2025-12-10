using Application.Abstractions.Messaging;

namespace Application.Features.Terminals.Commands.CreateTerminal
{
    public class CreateTerminalCommand : ICommand<Guid>
    {
        public Guid DepartmentId { get; set; }
        public string Name { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid? ManagerId { get; set; }

        // Adres Bilgileri (Department ile aynı yapı)
        public string Street { get; set; }
        public string BuildingNo { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string? Country { get; set; }
        public string? Floor { get; set; }
        public string? Door { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
