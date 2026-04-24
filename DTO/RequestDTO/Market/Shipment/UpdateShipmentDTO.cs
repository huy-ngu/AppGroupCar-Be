using AppGroupCar.Entities.Market;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.Shipment
{
    public class UpdateShipmentDTO
    {
        public Guid Id { get; set; }
        public ShipmentStatus? ShipmentStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
