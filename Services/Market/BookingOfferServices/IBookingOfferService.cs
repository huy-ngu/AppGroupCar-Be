using AppGroupCar.DTO.ResponseDTO.Market;

namespace AppGroupCar.Services.Market.BookingOfferServices
{
    public interface IBookingOfferService
    {
        Task<BookingOfferResponseDTO> CreateAsync(Guid RideRequestId, Guid OfferId);
        Task<IEnumerable<BookingOfferResponseDTO>> GetByRideRequestAsync(Guid RideRequestId);
        Task AcceptAsync(Guid BookingOfferId);
        Task RejectAsync(Guid BookingOfferId);
    }
}
