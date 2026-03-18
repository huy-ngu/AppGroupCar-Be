using AppGroupCar.DTO.RequestDTO.Bus.CompanyRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyResponse;
using AppGroupCar.Entities.Bus;
using AutoMapper;

namespace AppGroupCar.AutoMapper.Bus
{
    public class CompanyMapping : Profile
    {
        public CompanyMapping()
        {
            // Entity -> Response
            CreateMap<Company, CompanyResponseDto>();

            CreateMap<Company, CompanyDetailResponseDto>();

            // Request -> Entity
            CreateMap<CreateCompanyRequestDto, Company>()
                .ForMember(dest => dest.LogoUrl, opt => opt.Ignore());

            CreateMap<UpdateCompanyRequestDto, Company>()
                .ForMember(dest => dest.LogoUrl, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
