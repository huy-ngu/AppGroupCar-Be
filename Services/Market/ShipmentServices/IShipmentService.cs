using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;

namespace AppGroupCar.Services.Market.ShipmentServices
{
    public interface IShipmentService
    {
        Task UpdateAsync(Guid userId, UpdateShipmentDTO request);
        Task<IEnumerable<GetShipmentDTO>> GetMyShipmentsAsync(Guid userId);
        Task<GetShipmentDetailDTO> GetDetailByIdAsync(Guid userId, Guid shipmentId);
        Task<GetShipmentDetailDTO> GetDetailByOfferIdAsync(Guid userId, Guid shipmentOfferId);
        Task Compelete(Guid shipmentId);
        Task ReturnDelivery(Guid shipmentId);
        Task CancelAsync(Guid userId, Guid shipmentId);
        Task DriverUpdateStatusAsync(Guid userId, UpdateShipmentStatusDTO request);
    }
}
