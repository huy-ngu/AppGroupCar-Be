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
        public BookingCustomerInfoDTO? Customer { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public DateTime DepartureDate { get; set; }
        public string DriverName { get; set; }
        public string VehicleBrand { get; set; }
        public IEnumerable<RoutePointInfo> RoutePoints { get; set; }
        public IEnumerable<BookingItemBooking> Items { get; set; }

    }

    public class BookingCustomerInfoDTO
    {
        public Guid CustomerId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class RoutePointInfo
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
        public string StopType { get; set; }
        public int Sequence { get; set; }
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
