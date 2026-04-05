using AppGroupCar.DTO.RequestDTO.Market.BookingItem;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Services.Market.BookingItemServices
{
    public interface IBookingItemService
    {
        Task<BookingItem> CreateBookingItemAsync(CreateBookingItemRequestDTO request);
        Task<BookingItem> GetBookingItemByIdAsync(Guid id);
        Task<IEnumerable<BookingItem>> GetAllBookingItemsAsync();
        Task UpdateBookingItemAsync(UpdateBookingItemRequestDTO request);
        Task<bool> DeleteBookingItemAsync(Guid id);
    }
}
