using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.BookingItem
{
    public class CreateBookingItemRequestDTO
    {
        [Required]
        [DefaultValue("0ca37d6b-3392-4276-9d15-3016a90fc395")]

        public Guid BookingId { get; set; }

        [Required]
        [DefaultValue("a63177d3-2c21-48c8-bc9b-08de832bc18c")]
        public Guid PickupLocationId { get; set; }

        [Required]
        [DefaultValue("89c8eb10-273a-4987-bc9c-08de832bc18c")]
        public Guid DropoffLocationId { get; set; }

        [Required]
        [DefaultValue(100000)]
        public decimal Price { get; set; }
    }
}
