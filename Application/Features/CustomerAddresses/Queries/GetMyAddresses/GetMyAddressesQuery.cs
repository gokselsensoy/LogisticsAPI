using Application.Abstractions.Messaging;
using Application.Features.CustomerAddresses.DTOs;
using MediatR;

namespace Application.Features.CustomerAddresses.Queries.GetMyAddresses
{
    public class GetMyAddressesQuery : IRequest<List<AddressSelectionDto>>
    {
    }
}
