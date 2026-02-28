using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Market
{
    [Table("CargoOfferDetails")]
    public class CargoOfferDetail : BaseEntity
    {

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
