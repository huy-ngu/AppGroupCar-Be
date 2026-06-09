using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Services.Market.ShipmentOfferServices
{
    public interface IShipmentOfferService
    {
        Task<GetShipmentOfferDTO> CreateAsync(Guid driverId, CreateShipmentOfferRequestDTO request);
        Task<GetShipmentOfferDTO> CreateFromOfferAsync(CreateShipmentOfferFromOfferRequestDTO request);
        Task<IEnumerable<GetDetailShipmentOfferDTO>> GetByRequestId(Guid shipmentRequestId);
        Task<IEnumerable<GetPendingShipmentOfferDTO>> GetByOfferIdAsync(Guid driverId, Guid offerId);
        Task<IEnumerable<ShipmentOffer>> GetAll();
        Task<IEnumerable<GetDetailShipmentOfferDTO>> GetMyShipmentOffersAsync(Guid driverId);
        Task AcceptAsync(Guid shipmentOfferId, Guid UserId);
        Task DriverAcceptAsync(Guid driverId, Guid shipmentOfferId, Guid offerId);
        Task DriverRejectAsync(Guid driverId, Guid shipmentOfferId);
        Task CustomerReject(Guid customerId, Guid shipmentOfferId);
    }
}
