using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.BookingItem
{
    public class UpdateBookingItemRequestDTO
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? PickupLocationId { get; set; }

        public Guid? DropoffLocationId { get; set; }

        public decimal? Price { get; set; }
    }
}
