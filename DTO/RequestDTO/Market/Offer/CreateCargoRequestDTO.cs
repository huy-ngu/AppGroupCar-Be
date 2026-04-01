using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.Offer
{
    public class CreateCargoRequestDTO
    {
        [Required]
        [DefaultValue(5)]
        public decimal MaxWeight { get; set; }
        [Required]
        [DefaultValue(5)]
        public decimal MaxVolume { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool AcceptFragile { get; set; }
    }
}
