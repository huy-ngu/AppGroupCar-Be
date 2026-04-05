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

        public async Task<IEnumerable<BookingResponseDTO>> GetAllAsync()
        {
            var bookings = await _bookingRepository.GetQueryable()
                .Select(b => new BookingResponseDTO
                {
                    BookingId = b.Id,
                    OfferId = b.OfferId,
                    CustomerId = b.CustomerId,
                    Quantity = b.Quantity,
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
                    Quantity = b.Quantity,
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
                .FirstOrDefaultAsync();

            if (response == null) throw new AppException(404, "Không tìm thấy booking");

            return response;
        }

        public Task<IEnumerable<BookingResponseDTO>> BookingParameters()
        {
            throw new NotImplementedException();
        }

        public async Task<BookingResponseDTO> CreateWithBookingIteamsAsync(CreateWithBookingIteamsRequestDTO request)
        {
            if (request.ItemRequestDTOs == null)
            {
                throw new AppException(400, "Phải có bookingItems");
            }

            var offer = await _offerRepository.GetQueryable()
                        .Where(o => o.Id == request.OfferId)
                        .Where(o => o.DeleteAt == null)
                        .Include(o => o.SharedRideDetail)
                        .FirstOrDefaultAsync();

            if (offer == null)
            {
                throw new AppException(400, "Không tìm thấy offer");
            }

            int availableSeats = offer.SharedRideDetail.AvailableSeats;

            if (request.ItemRequestDTOs.Count() > availableSeats)
            {
                throw new AppException(400, "Không đủ ghế");
            }

            if (offer.DepartureTime < DateTime.UtcNow)
            {
                throw new AppException(400, "Chuyến xe đã xuất phát");
            }


            decimal totalprice = 0;

            foreach (var item in request.ItemRequestDTOs)
            {
                totalprice += item.Price;
            }

            var newBooking = new Booking
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                OfferId = request.OfferId,
                Status = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Unpaid,
                CreatedAt = DateTime.UtcNow,
                Quantity = request.ItemRequestDTOs.Count(),
                TotalPrice = totalprice,
                // Sử dụng Navigation Property thay vì dùng Repository con!
                Items = request.ItemRequestDTOs.Select(item => new BookingItem
                {
                    Id = Guid.NewGuid(),
                    Price = item.Price,
                    DropoffLocationId = item.DropoffLocationId,
                    PickupLocationId = item.PickupLocationId,

                }).ToList()
            };
            offer.SharedRideDetail.AvailableSeats = availableSeats - request.ItemRequestDTOs.Count();

            await _bookingRepository.AddAsync(newBooking);

            await _bookingRepository.SaveChangesAsync();
            return new BookingResponseDTO
            {
                BookingId = newBooking.Id,
                CustomerId = request.CustomerId,
                OfferId = request.OfferId,
                Quantity = request.ItemRequestDTOs.Count(),
                TotalPrice = totalprice
            };
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
