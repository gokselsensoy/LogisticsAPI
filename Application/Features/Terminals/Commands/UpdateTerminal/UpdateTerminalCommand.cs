using Application.Abstractions.Messaging;
using Application.Features.Terminals.Commands.CreateTerminal;

namespace Application.Features.Terminals.Commands.UpdateTerminal
{
    public class UpdateTerminalCommand : ICommand
    {
        public Guid TerminalId { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid? ManagerId { get; set; }

        // Adres
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
