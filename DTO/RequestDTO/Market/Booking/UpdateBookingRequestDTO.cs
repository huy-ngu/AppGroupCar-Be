using AppGroupCar.Entities.Market;
using System.ComponentModel;

namespace AppGroupCar.DTO.RequestDTO.Market.Booking
{
    public class UpdateBookingRequestDTO
    {
        [DefaultValue("0ca37d6b-3392-4276-9d15-3016a90fc395")]
        public Guid Id { get; set; }
        public BookingStatus? status { get; set; }
        public PaymentStatus? paymentStatus { get; set; }
        public decimal? TotalPrice { get; set; }

    }
}
