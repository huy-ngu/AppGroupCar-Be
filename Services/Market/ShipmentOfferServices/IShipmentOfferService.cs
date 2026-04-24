using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Services.Market.ShipmentOfferServices
{
    public interface IShipmentOfferService
    {
        Task<GetShipmentOfferDTO> CreateAsync(Guid driverId, CreateShipmentOfferRequestDTO request);
        Task<GetShipmentOfferDTO> CreateFromOfferAsync(Guid offerId, Guid requestId, decimal ProposedPrice);
        Task<IEnumerable<GetDetailShipmentOfferDTO>> GetByRequestId(Guid shipmentRequestId);
        Task<IEnumerable<ShipmentOffer>> GetAll();
        Task AcceptAsync(Guid shipmentOfferId, Guid UserId);
        Task DriverAcceptAsync(Guid driverId, Guid shipmentOfferId);
    }
}
