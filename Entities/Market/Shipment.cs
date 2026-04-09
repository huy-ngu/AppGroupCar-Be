using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Market
{
    [Table("Shipments")]
    public class Shipment : BaseEntity
    {
        public Guid? ShipmentOfferId { get; set; }
        [Required]
        public ShipmentStatus ShipmentStatus { get; set; }
        [Required]
        public PaymentStatus PaymentStatus { get; set; }
        public decimal Price { get; set; }
        public DateTime? DeliveredDate { get; set; }

        [ForeignKey(nameof(ShipmentOfferId))]
        public virtual ShipmentOffer? ShipmentOffer { get; set; }

    }

    public enum ShipmentStatus
    {
        Created = 1,
        InTransit = 2,
        Delivered = 3,
        Cancelled = 4,
        Return = 5
    }
}