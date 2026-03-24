using AppGroupCar.DTO.RequestDTO.Bus.UCRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.CUResponse;
using AppGroupCar.Entities.Bus;
using AutoMapper;

namespace AppGroupCar.AutoMapper.Bus
{
    public class CompanyUserProfile : Profile
    {
        public CompanyUserProfile()
        {
            CreateMap<
                CreateCURequestDto,
                CompanyUser>()
                .ForMember(
                    dest => dest.JoinedAt,
                    opt => opt.MapFrom(
                        src => DateTime.UtcNow))
                .ForMember(
                    dest => dest.IsActive,
                    opt => opt.MapFrom(
                        src => true));

            CreateMap<
                CompanyUser,
                CUResponseDto>();
        }
    }
}
