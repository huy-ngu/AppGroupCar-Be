using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Services.ShipmentRequestServices
{
    public interface IShipmentRequestServices
    {
        public Task<ShipmentRequest> CreateAsync(Guid userId, CreateShipmentRequestDTO request);
        public Task<ShipmentRequest> GetByIdAsync(Guid id);
        public Task DeleteAsync(Guid id);
    }
}
