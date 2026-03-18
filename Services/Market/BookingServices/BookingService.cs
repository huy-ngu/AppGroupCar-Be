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

        public BookingService(IBaseRepository<Booking> bookingRepository, IBaseRepository<BookingItem> bookingItemRepository)
        {
            _bookingRepository = bookingRepository;
            _bookingItemRepository = bookingItemRepository;
        }

        public async Task<BookingResponseDTO> CreateAsync(CreateBookingRequestDTO request)
        {
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

        public async Task<IEnumerable<BookingResponseDTO>> GetAllAsync()
        {
            var bookings = await _bookingRepository.GetQueryable()
                .Select(b => new BookingResponseDTO
                {
                    BookingId = b.Id,
                    OfferId = b.OfferId,
                    CustomerId = b.CustomerId,
                    Status = b.Status,
                    TotalPrice = b.TotalPrice,
                    PaymentStatus = b.PaymentStatus,
                    Items = b.Items.Select(i => new BookingItemBooking
                    {
                        ItemId = i.Id,
                        Price = i.Price,
                        PickupLocation = new LocationBooking
                        {
                            LocationId = i.PickupLocation.Id,
                            LocationName = i.PickupLocation.Name,
                        },
                        DropoffLocation = new LocationBooking
                        {
                            LocationId = i.DropoffLocation.Id,
                            LocationName = i.DropoffLocation.Name,
                        }

                    })
                }).ToListAsync();

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
                    Items = b.Items.Select(i => new BookingItemBooking
                    {
                        ItemId = i.Id,
                        Price = i.Price,
                        PickupLocation = new LocationBooking
                        {
                            LocationId = i.PickupLocation.Id,
                            LocationName = i.PickupLocation.Name
                        },

                        // Khởi tạo và ánh xạ trực tiếp object DropoffLocation
                        DropoffLocation = new LocationBooking
                        {
                            LocationId = i.DropoffLocation.Id,
                            LocationName = i.DropoffLocation.Name
                        }

                        // nếu nghiệp vụ cho phép droplocationID null thì dùng cái ở dưới
                        //DropoffLocation = i.DropoffLocation == null ? null : new LocationBooking
                        //{
                        //    LocationId = i.DropoffLocation.Id,
                        //    LocationName = i.DropoffLocation.Name
                        //}

                    }).ToList()
                })
                .FirstOrDefaultAsync(); // <--- TRUY VẤN XUỐNG DB CHẠY Ở ĐÂY!

            if (response == null) throw new AppException(404, "Không tìm thấy booking");

            return response;
        }

        public Task<IEnumerable<BookingResponseDTO>> BookingParameters()
        {
            throw new NotImplementedException();
        }

        public async Task CreateWithBookingIteamsAsync(CreateWithBookingIteamsRequestDTO request)
        {
            // 1. Khởi tạo Entity Cha và gán luôn danh sách Con vào nó
            var newBooking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                OfferId = request.OfferId,
                Status = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Unpaid,
                CreatedAt = DateTime.UtcNow,

                // Sử dụng Navigation Property thay vì dùng Repository con!
                Items = request.ItemRequestDTOs.Select(item => new BookingItem
                {
                    Id = Guid.NewGuid(),
                    // KHÔNG CẦN GÁN BookingId NỮA! EF Core sẽ tự động móc nối ID của Cha vào đây.
                    Price = item.Price,
                    DropoffLocationId = item.DropoffLocationId,
                    PickupLocationId = item.PickupLocationId
                }).ToList()
            };

            // 2. Thêm Cha vào DB (EF Core sẽ tự động theo dõi luôn cả đám Con bên trong)
            await _bookingRepository.AddAsync(newBooking);

            // 3. Gọi SaveChanges 1 lần duy nhất. EF Core tự động tạo Transaction, 
            // lưu Booking trước, lấy ID, nhét vào BookingItems rồi lưu tiếp BookingItems.
            await _bookingRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdateBookingRequestDTO request)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.Id);

            if (booking == null)
            {
                throw new AppException(404, "Không tìm thấy booking");
            }

            if (request.paymentStatus != null)
            {
                booking.PaymentStatus = (PaymentStatus)request.paymentStatus;
            }
            if (request.status != null)
            {
                booking.Status = (BookingStatus)request.status;
            }
            if (request.TotalPrice != null)
            {
                booking.TotalPrice = (decimal)request.TotalPrice;
            }

            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChangesAsync();
        }
    }
}
