using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Market
{
    [Table("Shipments")]
    public class Shipment : BaseEntity
    {
        public Guid? OfferId { get; set; }
        public Guid? ShipmentOfferId { get; set; }

        [Required]
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

        // --- NAVIGATION PROPERTIES ---

        [ForeignKey(nameof(OfferId))]
        public virtual Offer? Offer { get; set; }

        [ForeignKey(nameof(ShipmentOfferId))]
        public virtual ShipmentOffer? ShipmentOffer { get; set; }

        [ForeignKey(nameof(PickupLocationId))]
        public virtual Location PickupLocation { get; set; } = null!;

        [ForeignKey(nameof(DropoffLocationId))]
        public virtual Location DropoffLocation { get; set; } = null!;
    }

    public enum ShipmentStatus
    {
        Created = 1,
        InTransit = 2,
        Delivered = 3,
        Cancelled = 4
    }
}