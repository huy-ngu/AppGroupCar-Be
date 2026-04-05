using AppGroupCar.Data;
using AppGroupCar.DTO.RequestDTO.Market.BookingItem;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Market.BookingItemServices
{
    public class BookingItemService : IBookingItemService
    {
        private readonly IBaseRepository<BookingItem> _repo;

        public BookingItemService(IBaseRepository<BookingItem> repo)
        {
            _repo = repo;
        }

        public async Task<BookingItem> CreateBookingItemAsync(CreateBookingItemRequestDTO request)
        {
            var bookingItem = new BookingItem
            {
                Id = Guid.NewGuid(),
                BookingId = request.BookingId,
                PickupLocationId = request.PickupLocationId,
                DropoffLocationId = request.DropoffLocationId,
                Price = request.Price,
                CreatedAt = DateTime.UtcNow
            };

            _repo.AddAsync(bookingItem);
            await _repo.SaveChangesAsync();

            return bookingItem;
        }

        public async Task<BookingItem> GetBookingItemByIdAsync(Guid id)
        {
            var bookingItem = await _repo.GetByIdAsync(id);

            if (bookingItem == null)
            {
                throw new AppException(404, $"Không tìm thấy booking item");
            }

            return bookingItem;
        }

        public async Task<IEnumerable<BookingItem>> GetAllBookingItemsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task UpdateBookingItemAsync(UpdateBookingItemRequestDTO request)
        {
            var bookingItem = await _repo.GetQueryable().FirstOrDefaultAsync(bi => bi.Id == request.Id);

            if (bookingItem == null)
            {
                throw new AppException(404, $"Không tìm thấy booking item");
            }

            if (request.PickupLocationId.HasValue)
                bookingItem.PickupLocationId = request.PickupLocationId.Value;

            if (request.DropoffLocationId.HasValue)
                bookingItem.DropoffLocationId = request.DropoffLocationId.Value;

            if (request.Price.HasValue)
                bookingItem.Price = request.Price.Value;

            bookingItem.LastUpdateAt = DateTime.UtcNow;

            _repo.Update(bookingItem);
            await _repo.SaveChangesAsync();

            return;
        }

        public async Task<bool> DeleteBookingItemAsync(Guid id)
        {
            var bookingItem = await _repo.GetQueryable().FirstOrDefaultAsync(bi => bi.Id == id);

            if (bookingItem == null)
            {
                throw new AppException(404, $"Không tìm thấy booking item");
            }

            _repo.Delete(bookingItem);
            await _repo.SaveChangesAsync();

            return true;
        }
    }
}
