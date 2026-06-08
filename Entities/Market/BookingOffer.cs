using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Market
{
    public class BookingOffer : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Guid RideRequestId { get; set; }
        public BookingOfferStatus Status { get; set; }
        public Booking Booking { get; set; } = null!;
        public RideRequest RideRequest { get; set; } = null!;
        public enum BookingOfferStatus
        {
            Pending = 1,
            Accepted = 2,
            Rejected = 3
        }

    }
}
