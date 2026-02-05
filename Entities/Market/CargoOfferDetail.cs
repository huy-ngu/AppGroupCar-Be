using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Market
{
    [Table("CargoOfferDetails")]
    public class CargoOfferDetail
    {
        [Key]
        [ForeignKey(nameof(Offer))]
        public Guid OfferId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MaxWeight { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MaxVolume { get; set; }

        [Required]
        public bool AcceptFragile { get; set; }

        // Navigation
        public Offer Offer { get; set; } = null!;
    }

}
