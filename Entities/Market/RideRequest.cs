using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Market
{
    [Table("RideRequests")]
    public class RideRequest
    {
        [Key]
        public Guid RequestId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid FromLocationId { get; set; }

        [Required]
        public Guid ToLocationId { get; set; }

        [Required]
        public DateTime DesiredDepartureTime { get; set; }

        [Required]
        public int SeatsRequired { get; set; } = 1;

        [Required]
        public RideRequestStatus Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        [ForeignKey(nameof(CustomerId))]
        public User Customer { get; set; } = null!;

        [ForeignKey(nameof(FromLocationId))]
        public Location FromLocation { get; set; } = null!;

        [ForeignKey(nameof(ToLocationId))]
        public Location ToLocation { get; set; } = null!;
    }

    public enum RideRequestStatus
    {
        Open = 1,
        Matched = 2,
        Cancelled = 3
    }

}
