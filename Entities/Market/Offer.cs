using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Market
{
    [Table("Offers")]
    public class Offer
    {
        [Key]
        public Guid OfferId { get; set; }

        [Required]
        public string? DriverId { get; set; }

        [ForeignKey(nameof(DriverId))]
        public Driver Driver { get; set; } = null!;

        [Required]
        public Guid VehicleId { get; set; }

        [ForeignKey(nameof(VehicleId))]
        public Vehicle Vehicle { get; set; } = null!;

        [Required]
        public ServiceType ServiceType { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        [Required]
        public OfferStatus Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public ICollection<OfferRoutePoint> RoutePoints { get; set; }
            = new List<OfferRoutePoint>();

        public SharedRideOfferDetail? SharedRideDetail { get; set; }
        public CargoOfferDetail? CargoDetail { get; set; }
    }

    public enum OfferStatus
    {
        Draft = 1,
        Active = 2,
        Paused = 3,
        Closed = 4
    }

    public enum ServiceType
    {
        Shared = 1,
        Contract = 2,
        Truck = 3
    }

}
