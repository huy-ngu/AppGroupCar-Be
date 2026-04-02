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
        public Guid PickupLocationId { get; set; }
        public Guid DropoffLocationId { get; set; }
        public DateTime DeliveryDate { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Weight { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Volume { get; set; }
        public string? Description { get; set; }
        public bool IsFragile { get; set; } = false;
        public string? HandlingNote { get; set; }
        public string? ImageUrl { get; set; }
        public ShipmentStatus ShipmentStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

    }
}
