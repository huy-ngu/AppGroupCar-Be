using AppGroupCar.Entities.Market;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.ResponseDTO.Market
{
    public class BookingResponseDTO
    {
        public Guid BookingId { get; set; }
        public Guid OfferId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public IEnumerable<BookingItemBooking> Items { get; set; }

    }
    public class BookingItemBooking
    {
        public Guid ItemId { get; set; }
        public decimal Price { get; set; }
        public LocationBooking PickupLocation { get; set; }
        public LocationBooking DropoffLocation { get; set; }

    }

    public class LocationBooking
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
    }
}
