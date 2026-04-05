using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Services.Market.OfferRoutePointServices
{
    public interface IOfferRoutePointService
    {
        Task<IEnumerable<OfferRoutePoint>> GetAllAsync();
    }
}
