using AppGroupCar.Entities.Market;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.RequestDTO.Market.Shipment
{
    public class CreateShipmentFromOfferDTO
    {
        [Required]
        public Guid OfferId { get; set; }
        [Required]
        [DefaultValue("89C8EB10-273A-4987-BC9C-08DE832BC18C")]
        public Guid PickupLocationId { get; set; }

        [Required]
        public Guid DropoffLocationId { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Weight { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Volume { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public bool IsFragile { get; set; } = false;

        [MaxLength(500)]
        public string? HandlingNote { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public ShipmentStatus ShipmentStatus { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }
    }
}
