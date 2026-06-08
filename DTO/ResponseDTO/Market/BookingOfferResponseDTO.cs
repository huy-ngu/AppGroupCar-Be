using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market
{
    public class BookingOfferResponseDTO
    {
        public Guid Id { get; set; }
        public Guid OfferId { get; set; }
        public Guid RideRequestId { get; set; }
        public BookingOffer.BookingOfferStatus Status { get; set; }
        public Guid? BookingId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
