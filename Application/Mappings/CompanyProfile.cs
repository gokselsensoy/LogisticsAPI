using Application.Features.Suppliers.DTOs;
using AutoMapper;
using Domain.Entities.Companies;

namespace Application.Mappings
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<Supplier, SupplierDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.LogoUrl, opt => opt.Ignore()); // Logo varsa map'le
        }
    }
}
