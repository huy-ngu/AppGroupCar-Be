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
        Task<BookingResponseDTO> GetByIdAsync(Guid bookingId);
        Task<BookingResponseDTO> CreateAsync(CreateBookingRequestDTO request);
        Task CreateWithBookingIteamsAsync(CreateWithBookingIteamsRequestDTO request);
        Task<IEnumerable<BookingResponseDTO>> BookingParameters();
        Task UpdateAsync(UpdateBookingRequestDTO request);
    }
}
