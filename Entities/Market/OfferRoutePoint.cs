using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;
using AppGroupCar.Entities;

namespace AppGroupCar.Entities.Market
{
    [Table("OfferRoutePoints")]
    public class OfferRoutePoint : BaseEntity
    {
        [Required]
        public Guid OfferId { get; set; }

        [Required]
        public int Sequence { get; set; }

        [Required]
        public Guid LocationId { get; set; }

        [Required]
        public RouteStopType StopType { get; set; }

        [ForeignKey(nameof(OfferId))]
        public Offer Offer { get; set; } = null!;

        public Location Location { get; set; } = null!;
    }

}
