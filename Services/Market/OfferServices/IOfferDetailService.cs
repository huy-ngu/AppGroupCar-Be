using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Services.Market.OfferServices
{
    public interface IOfferDetailService
    {
        Task CreateAsync(CreateOfferWithDetailRequestDTO request);
        Task<IEnumerable<SharedRideOfferDetail>> GetAll();
    }
}
