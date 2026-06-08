using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Market
{
    public class BookingOffer : BaseEntity
    {
        public Guid OfferId { get; set; }
        public Guid RideRequestId { get; set; }
        public BookingOfferStatus Status { get; set; }
        
        public Guid? BookingId { get; set; }
        
        // Navigation properties
        public Offer Offer { get; set; } = null!;
        public RideRequest RideRequest { get; set; } = null!;
        public Booking? Booking { get; set; }

        public enum BookingOfferStatus
        {
            Pending = 1,
            Accepted = 2,
            Rejected = 3
        }
    }
}
