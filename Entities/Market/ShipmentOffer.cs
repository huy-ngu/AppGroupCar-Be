using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Market
{
    [Table("ShipmentOffers")]
    public class ShipmentOffer : BaseEntity
    {
        [Required]
        public Guid RequestId { get; set; }

        [Required]
        public Guid DriverId { get; set; }

        [Required]
        public Guid VehicleId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProposedPrice { get; set; }

        [Required]
        public ShipmentOfferStatus Status { get; set; }


        // Navigation
        [ForeignKey(nameof(RequestId))]
        public ShipmentRequest Request { get; set; } = null!;

        [ForeignKey(nameof(DriverId))]
        public Driver Driver { get; set; } = null!;

        [ForeignKey(nameof(VehicleId))]
        public Vehicle Vehicle { get; set; } = null!;

        public Shipment? Shipment { get; set; }
    }

    public enum ShipmentOfferStatus
    {
        Pending = 1,
        Accepted = 2,
        Rejected = 3
    }

}
