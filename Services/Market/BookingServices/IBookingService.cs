using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.Booking;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Services.Market.BookingServices
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponseDTO>> GetAllAsync();
        Task<IEnumerable<BookingResponseDTO>> GetMyBookingsAsync(Guid customerId);
        Task<IEnumerable<BookingResponseDTO>> GetPendingByOfferIdAsync(Guid driverId, Guid offerId);
        Task<BookingResponseDTO> GetByIdAsync(Guid bookingId);
        //Task<BookingResponseDTO> CreateAsync(CreateBookingRequestDTO request);
        Task<BookingResponseDTO> CreateWithBookingIteamsAsync(Guid customerId, CreateWithBookingIteamsRequestDTO request);
        Task<IEnumerable<BookingResponseDTO>> BookingParameters();
        Task UpdateAsync(UpdateBookingRequestDTO request);
        Task CancelAsync(Guid customerId, Guid bookingId);
        Task AcceptBookingAsync(Guid driverId, Guid bookingId);
        Task RejectBookingAsync(Guid driverId, Guid bookingId);
    }
}
