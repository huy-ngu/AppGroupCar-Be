using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Market
{
    [Table("Offers")]
    public class Offer : BaseEntity
    {
        [Required]
        public Guid DriverId { get; set; }

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
        public ICollection<OfferRoutePoint> RoutePoints { get; set; }
            = new List<OfferRoutePoint>();

        public SharedRideOfferDetail? SharedRideDetail { get; set; }
        public CargoOfferDetail? CargoDetail { get; set; }
    }

    public enum OfferStatus
    {
        Active = 1,
        Paused = 2,
        Closed = 3,
        Complete = 4,
    }

    public enum ServiceType
    {
        Shared = 1,
        Contract = 2,
        Truck = 3,
        Bus = 4
    }

}
