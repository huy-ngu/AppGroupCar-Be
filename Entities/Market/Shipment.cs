using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Market
{
    [Table("Shipments")]
    public class Shipment
    {
        [Key]
        public Guid ShipmentId { get; set; }

        [Required]
        public Guid ShipmentOfferId { get; set; }

        [Required]
        public ShipmentStatus ShipmentStatus { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // Navigation
        [ForeignKey(nameof(ShipmentOfferId))]
        public ShipmentOffer ShipmentOffer { get; set; } = null!;
    }

    public enum ShipmentStatus
    {
        Created = 1,
        InTransit = 2,
        Delivered = 3,
        Cancelled = 4
    }

}
