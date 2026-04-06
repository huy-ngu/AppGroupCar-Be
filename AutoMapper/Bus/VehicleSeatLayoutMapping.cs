using AppGroupCar.DTO.RequestDTO.Bus.SeatLayoutRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.SeatLayoutResponse;
using AppGroupCar.Entities.Bus;
using AutoMapper;
using System.Text.Json;

namespace AppGroupCar.AutoMapper.Bus
{
    public class VehicleSeatLayoutMapping : Profile
    {
        public VehicleSeatLayoutMapping()
        {
            CreateMap<CreateVehicleSeatLayoutRequest, VehicleSeatLayout>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.LayoutJson, opt => opt.MapFrom(src => JsonSerializer.Serialize(src.LayoutJson, (JsonSerializerOptions)null)));

            CreateMap<VehicleSeatLayout, VehicleSeatLayoutResponseDto>()
                .ForMember(d => d.LayoutJson, opt => opt.MapFrom(src => JsonSerializer.Deserialize<JsonElement>(src.LayoutJson, (JsonSerializerOptions)null)));
        }
    }
}
