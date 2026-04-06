using AppGroupCar.DTO.RequestDTO.Bus.RouteRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.RouteResponse;
using AppGroupCar.Entities.Bus;
using AutoMapper;

namespace AppGroupCar.AutoMapper.Bus
{
    public class BusRouteMapping : Profile
    {
        public BusRouteMapping()
        {
            CreateMap<BusRoute, BusRouteResponse>();

            CreateMap<BusRoutePoint, BusRoutePointResponse>()
                .ForMember(dest => dest.LocationName,
                    opt => opt.MapFrom(src => src.Location.Name));

            CreateMap<CreateBusRouteRequest, BusRoute>();

            CreateMap<CreateBusRoutePointRequest, BusRoutePoint>();
        }
    }
}
