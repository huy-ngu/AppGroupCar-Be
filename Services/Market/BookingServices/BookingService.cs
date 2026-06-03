using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.Booking;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities;
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
        private readonly IBaseRepository<SharedRideOfferDetail> _sharedRideOfferDetailRepository;

        public BookingService(
            IBaseRepository<Booking> bookingRepository,
            IBaseRepository<BookingItem> bookingItemRepository,
            IBaseRepository<Offer> offerRepository,
            IBaseRepository<SharedRideOfferDetail> sharedRideOfferDetailRepository)
        {
            _bookingRepository = bookingRepository;
            _bookingItemRepository = bookingItemRepository;
            _offerRepository = offerRepository;
            _sharedRideOfferDetailRepository = sharedRideOfferDetailRepository;
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetAllAsync()
        {
            var bookings = await _bookingRepository.GetQueryable()
                .Where(b => b.DeleteAt == null)
                .Include(b => b.Offer)
                .ThenInclude(o => o.Driver)
                .Include(b => b.Offer)
                .ThenInclude(o => o.Vehicle)
                .Include(b => b.Offer)
                .ThenInclude(o => o.RoutePoints)
                .ThenInclude(rp => rp.Location)
                .Select(b => new BookingResponseDTO
                {
                    BookingId = b.Id,
                    OfferId = b.OfferId,
                    CustomerId = b.CustomerId,
                    Quantity = b.Quantity,
                    Status = b.Status,
                    TotalPrice = b.TotalPrice,
                    PaymentStatus = b.PaymentStatus,
                    DepartureDate = b.Offer.DepartureTime,
                    DriverName = b.Offer.Driver.Name,
                    VehicleBrand = b.Offer.Vehicle.Brand,
                    RoutePoints = b.Offer.RoutePoints
                        .Where(rp => rp.StopType == RouteStopType.Start || rp.StopType == RouteStopType.End)
                        .Select(rp => new RoutePointInfo
                        {
                            LocationId = rp.LocationId,
                            LocationName = rp.Location.Name,
                            StopType = rp.StopType.ToString(),
                            Sequence = rp.Sequence
                        }),
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
                .Include(b => b.Offer)
                .ThenInclude(o => o.Driver)
                .Include(b => b.Offer)
                .ThenInclude(o => o.Vehicle)
                .Include(b => b.Offer)
                .ThenInclude(o => o.RoutePoints)
                .ThenInclude(rp => rp.Location)
                .Select(b => new BookingResponseDTO
                {
                    BookingId = b.Id,
                    OfferId = b.OfferId,
                    CustomerId = b.CustomerId,
                    Quantity = b.Quantity,
                    Status = b.Status,
                    TotalPrice = b.TotalPrice,
                    PaymentStatus = b.PaymentStatus,
                    DepartureDate = b.Offer.DepartureTime,
                    DriverName = b.Offer.Driver.Name,
                    VehicleBrand = b.Offer.Vehicle.Brand,
                    RoutePoints = b.Offer.RoutePoints
                        .Where(rp => rp.StopType == RouteStopType.Start || rp.StopType == RouteStopType.End)
                        .Select(rp => new RoutePointInfo
                        {
                            LocationId = rp.LocationId,
                            LocationName = rp.Location.Name,
                            StopType = rp.StopType.ToString(),
                            Sequence = rp.Sequence
                        }),
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
                .Include(b => b.Offer)
                .ThenInclude(o => o.Driver)
                .Include(b => b.Offer)
                .ThenInclude(o => o.Vehicle)
                .Include(b => b.Offer)
                .ThenInclude(o => o.RoutePoints)
                .ThenInclude(rp => rp.Location)
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
                    DepartureDate = b.Offer.DepartureTime,
                    DriverName = b.Offer.Driver.Name,
                    VehicleBrand = b.Offer.Vehicle.Brand,
                    RoutePoints = b.Offer.RoutePoints
                        .Where(rp => rp.StopType == RouteStopType.Start || rp.StopType == RouteStopType.End)
                        .Select(rp => new RoutePointInfo
                        {
                            LocationId = rp.LocationId,
                            LocationName = rp.Location.Name,
                            StopType = rp.StopType.ToString(),
                            Sequence = rp.Sequence
                        })
                        .ToList(),
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

        public async Task<BookingResponseDTO> CreateWithBookingIteamsAsync(Guid customerId, CreateWithBookingIteamsRequestDTO request)
        {
            if (request == null)
            {
                throw new AppException(400, "Request không hợp lệ");
            }

            if (customerId == Guid.Empty)
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
                        .Include(o => o.Driver)
                        .Include(o => o.Vehicle)
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
                CustomerId = customerId,
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
            //offer.SharedRideDetail.AvailableSeats = availableSeats - bookingItems.Count;

            await _bookingRepository.AddAsync(newBooking);

            await _bookingRepository.SaveChangesAsync();
            return new BookingResponseDTO
            {
                BookingId = newBooking.Id,
                CustomerId = customerId,
                OfferId = request.OfferId,
                Quantity = bookingItems.Count,
                TotalPrice = totalprice,
                DepartureDate = offer.DepartureTime,
                DriverName = offer.Driver.Name,
                VehicleBrand = offer.Vehicle.Brand,
                RoutePoints = offer.RoutePoints
                    .Where(rp => rp.StopType == RouteStopType.Start || rp.StopType == RouteStopType.End)
                    .Select(rp => new RoutePointInfo
                    {
                        LocationId = rp.LocationId,
                        LocationName = rp.Location.Name,
                        StopType = rp.StopType.ToString(),
                        Sequence = rp.Sequence
                    })
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

        public async Task CancelAsync(Guid customerId, Guid bookingId)
        {
            if (customerId == Guid.Empty || bookingId == Guid.Empty)
            {
                throw new AppException(400, "ID không hợp lệ");
            }

            var booking = await _bookingRepository.GetQueryable()
                .Include(b => b.Offer)
                .ThenInclude(o => o.SharedRideDetail)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.DeleteAt == null)
                ?? throw new AppException(404, "Không tìm thấy booking");

            if (booking.CustomerId != customerId)
            {
                throw new AppException(403, "Bạn không được phép hủy booking này");
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                throw new AppException(400, "Booking đã bị hủy");
            }

            //if (booking.Status == BookingStatus.Confirmed)
            //{
            //    throw new AppException(400, "Không thể hủy booking đã được xác nhận");
            //}

            if (booking.Offer == null)
            {
                throw new AppException(404, "Không tìm thấy thông tin offer");
            }

            if (booking.Offer.SharedRideDetail == null)
            {
                throw new AppException(404, "Không tìm thấy thông tin chi tiết chuyến xe");
            }

            booking.Status = BookingStatus.Cancelled;
            _bookingRepository.Update(booking);

            var sharedRideDetail = booking.Offer.SharedRideDetail;
            sharedRideDetail.AvailableSeats += booking.Quantity;
            _sharedRideOfferDetailRepository.Update(sharedRideDetail);

            await _bookingRepository.SaveChangesAsync();
        }

        public async Task AcceptBookingAsync(Guid driverId, Guid bookingId)
        {
            if (driverId == Guid.Empty || bookingId == Guid.Empty)
            {
                throw new AppException(400, "ID không hợp lệ");
            }

            var booking = await _bookingRepository.GetQueryable()
                .Include(b => b.Offer)
                .ThenInclude(o => o.SharedRideDetail)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.DeleteAt == null)
                ?? throw new AppException(404, "Không tìm thấy booking");

            // Kiểm tra driver có quyền chấp nhận booking này không
            if (booking.Offer.DriverId != driverId)
            {
                throw new AppException(403, "Bạn không được phép chấp nhận booking này");
            }

            // Kiểm tra trạng thái booking phải là Pending
            if (booking.Status != BookingStatus.Pending)
            {
                throw new AppException(400, "Chỉ có thể chấp nhận booking ở trạng thái chờ");
            }

            if (booking.Offer == null)
            {
                throw new AppException(404, "Không tìm thấy thông tin offer");
            }

            if (booking.Offer.SharedRideDetail == null)
            {
                throw new AppException(404, "Không tìm thấy thông tin chi tiết chuyến xe");
            }

            // Kiểm tra còn ghế trống không
            if (booking.Offer.SharedRideDetail.AvailableSeats < booking.Quantity)
            {
                throw new AppException(400, "Không đủ ghế để chấp nhận booking");
            }

            // Cập nhật trạng thái booking thành Accepted
            booking.Status = BookingStatus.Accepted;
            _bookingRepository.Update(booking);

            // Trừ số ghế còn lại
            var sharedRideDetail = booking.Offer.SharedRideDetail;
            sharedRideDetail.AvailableSeats -= booking.Quantity;
            _sharedRideOfferDetailRepository.Update(sharedRideDetail);

            await _bookingRepository.SaveChangesAsync();
        }

        public async Task RejectBookingAsync(Guid driverId, Guid bookingId)
        {
            if (driverId == Guid.Empty || bookingId == Guid.Empty)
            {
                throw new AppException(400, "ID không hợp lệ");
            }

            var booking = await _bookingRepository.GetQueryable()
                .Include(b => b.Offer)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.DeleteAt == null)
                ?? throw new AppException(404, "Không tìm thấy booking");

            // Kiểm tra driver có quyền từ chối booking này không
            if (booking.Offer.DriverId != driverId)
            {
                throw new AppException(403, "Bạn không được phép từ chối booking này");
            }

            // Kiểm tra trạng thái booking phải là Pending
            if (booking.Status != BookingStatus.Pending)
            {
                throw new AppException(400, "Chỉ có thể từ chối booking ở trạng thái chờ");
            }

            // Cập nhật trạng thái booking thành Rejected
            booking.Status = BookingStatus.Rejected;
            _bookingRepository.Update(booking);

            await _bookingRepository.SaveChangesAsync();
        }
    }
}
