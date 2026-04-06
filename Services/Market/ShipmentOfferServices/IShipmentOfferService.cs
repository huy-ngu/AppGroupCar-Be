using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Services.Market.ShipmentOfferServices
{
    public interface IShipmentOfferService
    {
        Task<GetShipmentOfferDTO> CreateAsync(Guid driverId, CreateShipmentOfferRequestDTO request);
        Task<GetShipmentOfferDTO> CreateFromOfferAsync(Guid offerId, decimal ProposedPrice);
        Task<IEnumerable<GetShipmentOfferDTO>> GetByRequestId(Guid shipmentRequestId);
        Task<IEnumerable<ShipmentOffer>> GetAll();
        Task AcceptAsync(Guid shipmentOfferId);
    }
}
