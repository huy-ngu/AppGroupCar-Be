using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;

namespace AppGroupCar.Services.Market.ShipmentServices
{
    public class ShipmentService(
            IBaseRepository<Shipment> _shipmentRepo
        ) : IShipmentService
    {
        public async Task UpdateAsync(UpdateShipmentDTO request)
        {
            var shipment = await _shipmentRepo.GetByIdAsync(request.Id) ?? throw new AppException(404, "Không tìm thấy bản ghi");
            if (request.ShipmentStatus is not null) shipment.ShipmentStatus = (ShipmentStatus)request.ShipmentStatus;

            if (request.PaymentStatus is not null) { shipment.PaymentStatus = shipment.PaymentStatus; }

            await _shipmentRepo.SaveChangesAsync();
        }
    }
}
