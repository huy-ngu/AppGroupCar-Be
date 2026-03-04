using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.ShipmentRequest;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Services.Market.ShipmentRequestServices
{
    public interface IShipmentRequestServices
    {
        public Task<ShipmentRequest> CreateAsync(Guid userId, CreateShipmentRequestDTO request);
        public Task<ShipmentRequest> GetByIdAsync(Guid id);
        public Task DeleteAsync(Guid id);
        public Task UpdateAsync(UpdateShipmentRequestDTO request);
        public Task<PagedResult<ShipmentRequest>> FilterAsync(ShipmentRequestParameters param);
    }
}
