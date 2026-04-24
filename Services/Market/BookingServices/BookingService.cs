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
                .Where(b => b.DeleteAt == null)
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
        public async Task<IEnumerable<BookingResponseDTO>> GetMyBookingsAsync(Guid customerId)
        {
            if (customerId == Guid.Empty) throw new AppException(400, "ID không hợp lệ");

            var bookings = await _bookingRepository.GetQueryable()
                .Where(b => b.CustomerId == customerId && b.DeleteAt == null)
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
                .Where(b => b.Id == bookingId && b.DeleteAt == null)
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
            if (request == null)
            {
                throw new AppException(400, "Request không hợp lệ");
            }

            if (request.CustomerId == Guid.Empty)
            {
                throw new AppException(400, "CustomerId không hợp lệ");
            }

            if (request.ItemRequestDTOs == null)
            {
                throw new AppException(400, "Phải có bookingItems");
            }

            var bookingItems = request.ItemRequestDTOs.ToList();
            if (!bookingItems.Any())
            {
                throw new AppException(400, "Phải có ít nhất 1 booking item");
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

            if (offer.SharedRideDetail == null)
            {
                throw new AppException(400, "Offer không có thông tin SharedRideDetail");
            }

            int availableSeats = offer.SharedRideDetail.AvailableSeats;

            if (bookingItems.Count > availableSeats)
            {
                throw new AppException(400, "Không đủ ghế");
            }

            if (offer.DepartureTime < DateTime.UtcNow)
            {
                throw new AppException(400, "Chuyến xe đã xuất phát");
            }


            decimal totalprice = 0;
            foreach (var item in bookingItems)
            {
                if (item.PickupLocationId == Guid.Empty || item.DropoffLocationId == Guid.Empty)
                {
                    throw new AppException(400, "PickupLocationId hoặc DropoffLocationId không hợp lệ");
                }
                if (item.Price <= 0)
                {
                    throw new AppException(400, "Giá tiền phải lớn hơn 0");
                }
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
                Quantity = bookingItems.Count,
                TotalPrice = totalprice,
                Items = bookingItems.Select(item => new BookingItem
                {
                    Id = Guid.NewGuid(),
                    Price = item.Price,
                    DropoffLocationId = item.DropoffLocationId,
                    PickupLocationId = item.PickupLocationId,
                }).ToList()
            };
            offer.SharedRideDetail.AvailableSeats = availableSeats - bookingItems.Count;

            await _bookingRepository.AddAsync(newBooking);

            await _bookingRepository.SaveChangesAsync();
            return new BookingResponseDTO
            {
                BookingId = newBooking.Id,
                CustomerId = request.CustomerId,
                OfferId = request.OfferId,
                Quantity = bookingItems.Count,
                TotalPrice = totalprice
            };
        }

        public async Task UpdateAsync(UpdateBookingRequestDTO request)
        {
            if (request.Id == Guid.Empty)
            {
                throw new AppException(400, "ID không hợp lệ");
            }

            var booking = await _bookingRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.DeleteAt == null);

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
