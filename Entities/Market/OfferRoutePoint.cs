using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Market
{
    [Table("OfferRoutePoints")]
    public class OfferRoutePoint
    {
        [Required]
        public Guid OfferId { get; set; }

        [Required]
        public int Sequence { get; set; }

        [Required]
        public Guid LocationId { get; set; }

        [Required]
        public bool PickupAllowed { get; set; }

        [Required]
        public bool DropoffAllowed { get; set; }

        [ForeignKey(nameof(OfferId))]
        public Offer Offer { get; set; } = null!;
    }

}
