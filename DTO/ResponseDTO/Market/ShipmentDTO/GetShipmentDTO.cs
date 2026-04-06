using AppGroupCar.Entities.Market;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO
{
    public class GetShipmentDTO
    {
        public Guid Id { get; set; }
        public Guid? OfferId { get; set; }
        public Guid? ShipmentOfferId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public ShipmentStatus ShipmentStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

    }
}
