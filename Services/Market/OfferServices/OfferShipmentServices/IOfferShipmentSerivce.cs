using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO.Market.Offer;

namespace AppGroupCar.Services.Market.OfferServices.OfferShipmentServices
{
    public interface IOfferShipmentSerivce
    {
        Task<GetOfferDTO> CreateWithCargoAsync(Guid driverId, CreateOfferShipmentRequestDTO request);
        Task<PagedResult<GetOfferShipmentDTO>> GetOfferShipmentAsync(GetOfferForUserRequestDTO request);

    }
}
