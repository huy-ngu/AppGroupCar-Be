using AppGroupCar.DTO.ResponseDTO.Market.Offer;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market
{
    public class BookingOfferDetailResponseDTO
    {
        public Guid Id { get; set; }
        public Guid OfferId { get; set; }
        public Guid RideRequestId { get; set; }
        public BookingOffer.BookingOfferStatus Status { get; set; }
        public Guid? BookingId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public GetMarketDriverDetailDTO Driver { get; set; } = null!;
        public GetOfferDTO Offer { get; set; } = null!;
    }
}
