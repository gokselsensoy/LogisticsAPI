using Application.Abstractions.Services;
using Domain.Repositories;
using MediatR;

namespace Application.Features.CustomerAddresses.DTOs
{
    public class AddressSelectionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } // "Ev", "Merkez Depo"
        public string FormattedAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
