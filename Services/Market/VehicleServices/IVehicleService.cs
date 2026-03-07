using AppGroupCar.DTO.RequestDTO.Market.Vehicle;
using AppGroupCar.DTO.ResponseDTO.Market;

namespace AppGroupCar.Services.Market.VehicleServices
{
    public interface IVehicleService
    {
        Task<VehicleDTO> CreateAsync(Guid driverId, CreateVehicleDTO dto);
        Task<IEnumerable<VehicleDTO>> GetByDriverIdAsync(Guid driverId);
        Task<VehicleDTO> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, Guid driverId, UpdateVehicleDTO dto);
        Task DeleteAsync(Guid id, Guid driverId);
    }
}
