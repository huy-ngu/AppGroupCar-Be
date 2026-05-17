using AppGroupCar.DTO.RequestDTO.Bus.CompanyDriverRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyDriverResponse;
using AppGroupCar.Entities.Bus;
using AutoMapper;

namespace AppGroupCar.AutoMapper.Bus
{
    public class CompanyDriverMapping : Profile
    {
        public CompanyDriverMapping()
        {
            CreateMap<CreateCompanyDriverRequestDto, CompanyDriver>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CompanyId, opt => opt.Ignore());

            CreateMap<UpdateCompanyDriverRequestDto, CompanyDriver>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CompanyId, opt => opt.Ignore())
                .ForMember(d => d.UserId, opt => opt.Ignore());

            CreateMap<CompanyDriver, CompanyDriverResponseDto>();
            
            CreateMap<CompanyDriver, CompanyDriverDetailResponseDto>()
                .ForMember(d => d.UserFullName, opt => opt.MapFrom(s => s.User.FullName))
                .ForMember(d => d.UserEmail, opt => opt.MapFrom(s => s.User.Email))
                .ForMember(d => d.CompanyName, opt => opt.MapFrom(s => s.Company.CompanyName));
        }
    }
}
