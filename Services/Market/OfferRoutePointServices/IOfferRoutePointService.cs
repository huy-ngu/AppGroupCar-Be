using AppGroupCar.DTO.RequestDTO.Market.OfferRoutePoint;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Services.Market.OfferRoutePointServices
{
    public interface IOfferRoutePointService
    {
        Task<IEnumerable<OfferRoutePoint>> GetAllAsync();
        Task<IEnumerable<GetOfferRoutePointDTO>> GetByOfferIdOrderedAsync(Guid offerId);
        Task<OfferRoutePoint> CreateAsync(CreateOfferRoutePointRequestDTO request);
        Task UpdateAsync(Guid id, UpdateOfferRoutePointRequestDTO request);
        Task<bool> DeleteAsync(Guid id);
    }
}
