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

        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
        public string? ImageUrl { get; set; }
        public AppGroupCar.DTO.ResponseDTO.Market.Offer.LocationDetailDTO PickupPoint { get; set; } = null!;
        public AppGroupCar.DTO.ResponseDTO.Market.Offer.LocationDetailDTO DropoffPoint { get; set; } = null!;
    }
}
