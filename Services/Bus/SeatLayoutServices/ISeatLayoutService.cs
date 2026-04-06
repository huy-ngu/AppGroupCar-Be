using AppGroupCar.DTO.RequestDTO.Bus.SeatLayoutRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.SeatLayoutResponse;

namespace AppGroupCar.Services.Bus.SeatLayoutServices
{
    public interface ISeatLayoutService
    {
        Task<VehicleSeatLayoutResponseDto> CreateAsync(CreateVehicleSeatLayoutRequest dto);
        Task<IEnumerable<VehicleSeatLayoutResponseDto>> GetAllAsync();
    }
}
