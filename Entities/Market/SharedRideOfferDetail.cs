using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Market
{
    [Table("SharedRideOfferDetails")]
    public class SharedRideOfferDetail : BaseEntity
    {
        [Required]
        public int TotalSeats { get; set; }

        [Required]
        public int AvailableSeats { get; set; }

        // Navigation
        public Offer Offer { get; set; } = null!;
    }
}
