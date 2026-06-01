using AppGroupCar.DTO.RequestDTO.Market.BookingItem;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.Booking
{
    public class CreateWithBookingIteamsRequestDTO
    {
        [DefaultValue("3de6663e-4016-4278-4b4a-08de7f263702")]
        public Guid OfferId { get; set; }
        //public IEnumerable<CreateBookingItemRequestDTO> ItemRequestDTOs { get; set; }
        public IEnumerable<BookingItemRequestDTO> ItemRequestDTOs { get; set; }
    }
    public class BookingItemRequestDTO
    {
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
