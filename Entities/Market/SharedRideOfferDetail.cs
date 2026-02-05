using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Market
{
    [Table("SharedRideOfferDetails")]
    public class SharedRideOfferDetail
    {
        [Key]
        [ForeignKey(nameof(Offer))]
        public Guid OfferId { get; set; }

        [Required]
        public int TotalSeats { get; set; }

        [Required]
        public int AvailableSeats { get; set; }

        // Navigation
        public Offer Offer { get; set; } = null!;
    }
}
