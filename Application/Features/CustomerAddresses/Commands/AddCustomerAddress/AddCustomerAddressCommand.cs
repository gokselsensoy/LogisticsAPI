using Application.Abstractions.Messaging;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Features.CustomerAddresses.Commands.AddCustomerAddress
{
    public class AddCustomerAddressCommand : ICommand<Guid>
    {
        public string Title { get; set; }
        public string Street { get; set; }
        public string BuildingNo { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string? Country { get; set; }
        public string? Floor { get; set; }
        public string? Door { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public AddressType Type { get; set; }
    }
}
