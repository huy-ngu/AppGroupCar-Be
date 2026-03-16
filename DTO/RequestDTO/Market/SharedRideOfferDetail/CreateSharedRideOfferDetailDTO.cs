using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.SharedRideOfferDetail
{
    public class CreateSharedRideOfferDetailDTO
    {
        [Required]
        public Guid OfferId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "TotalSeats phải lớn hơn 0")]
        [DefaultValue(1)]

        public int TotalSeats { get; set; }

        [Required]
        [DefaultValue(100000)]
        [Range(0, int.MaxValue, ErrorMessage = "AvailableSeats không âm")]
        public int AvailableSeats { get; set; }
    }
}
