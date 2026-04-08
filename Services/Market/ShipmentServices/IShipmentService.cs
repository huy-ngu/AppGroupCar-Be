using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;

namespace AppGroupCar.Services.Market.ShipmentServices
{
    public interface IShipmentService
    {
        Task UpdateAsync(UpdateShipmentDTO request);
    }
}
