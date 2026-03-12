using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.SharedRideOfferDetail
{
    public class UpdateSharedRideOfferDetailDTO
    {
        public int? TotalSeats { get; set; }

        public int? AvailableSeats { get; set; }
    }
}
