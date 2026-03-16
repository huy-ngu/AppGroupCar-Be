using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.Booking;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppGroupCar.Services.Market.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly IBaseRepository<Booking> _bookingRepository;
        private readonly IBaseRepository<BookingItem> _bookingItemRepository;
        private readonly IBaseRepository<Offer> _offerRepository;

        public BookingService(IBaseRepository<Booking> bookingRepository, IBaseRepository<BookingItem> bookingItemRepository, IBaseRepository<Offer> offerRepository)
        {
            _bookingRepository = bookingRepository;
            _bookingItemRepository = bookingItemRepository;
            _offerRepository = offerRepository;
        }

        public async Task<BookingResponseDTO> CreateAsync(CreateBookingRequestDTO request)
        {
            var offer = await _offerRepository.GetByIdAsync(request.OfferId);

            //if (offer == null || offer.DeleteAt != null)
            //{
            //    throw new AppException(404, "Không tìm thấy offer");
            //}

            var newBooking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                OfferId = request.OfferId,
                Status = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Unpaid,
                CreatedAt = DateTime.UtcNow,
            };
            await _bookingRepository.AddAsync(newBooking);
            await _bookingRepository.SaveChangesAsync();
            return new BookingResponseDTO
            {
                BookingId = newBooking.Id,
                CustomerId = request.CustomerId,
                OfferId = request.OfferId,
            };
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings;
        }

        public async Task<BookingResponseDTO> GetByIdAsync(Guid bookingId)
        {
            if (bookingId == Guid.Empty) throw new AppException(400, "ID không hợp lệ");

            // Dùng .Select() NGAY TRÊN IQueryable
            var response = await _bookingRepository.GetQueryable()
                .Where(b => b.Id == bookingId)
                // Áp dụng luôn Mapping vào thẳng câu SQL
                .Select(b => new BookingResponseDTO
                {
                    BookingId = b.Id,
                    OfferId = b.OfferId,
                    CustomerId = b.CustomerId,
                    Status = b.Status,
                    TotalPrice = b.TotalPrice,
                    PaymentStatus = b.PaymentStatus,
                    Items = b.Items.Select(i => new BookingItemResponseDTO
                    {
                        ItemId = i.Id,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList()
                })
                .FirstOrDefaultAsync(); // <--- TRUY VẤN XUỐNG DB CHẠY Ở ĐÂY!

            if (response == null) throw new AppException(404, "Không tìm thấy booking");

            return response;
        }


    }
}
