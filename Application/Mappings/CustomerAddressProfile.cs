using Application.Features.CustomerAddresses.DTOs;
using AutoMapper;
using Domain.Entities.Customers;

namespace Application.Mappings
{
    public class CustomerAddressProfile : Profile
    {
        public CustomerAddressProfile()
        {
            CreateMap<CustomerAddress, AddressSelectionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.FormattedAddress, opt => opt.MapFrom(src => src.Address.FormattedAddress))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Address.Location.Y))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Address.Location.X));

            CreateMap<CorporateAddressResponsibleMap, AddressSelectionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CustomerAddress.Id)) // Map içindeki AddressId değil, Nav property üzerinden
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.CustomerAddress.Title))
                .ForMember(dest => dest.FormattedAddress, opt => opt.MapFrom(src => src.CustomerAddress.Address.FormattedAddress))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.CustomerAddress.Address.Location.Y))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.CustomerAddress.Address.Location.X));
        }
    }
}
