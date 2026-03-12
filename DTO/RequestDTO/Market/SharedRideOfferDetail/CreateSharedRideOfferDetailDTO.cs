using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.SharedRideOfferDetail
{
    public class CreateSharedRideOfferDetailDTO
    {
        [Required(ErrorMessage = "OfferId là b?t bu?c")]
        public Guid OfferId { get; set; }

        [Required(ErrorMessage = "TotalSeats là b?t bu?c")]
        [Range(1, int.MaxValue, ErrorMessage = "TotalSeats ph?i l?n h?n 0")]
        public int TotalSeats { get; set; }

        [Required(ErrorMessage = "AvailableSeats là b?t bu?c")]
        [Range(0, int.MaxValue, ErrorMessage = "AvailableSeats không th? âm")]
        public int AvailableSeats { get; set; }
    }
}
