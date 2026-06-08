using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;
using static AppGroupCar.Entities.Market.BookingOffer;

namespace AppGroupCar.Services.Market.BookingOfferServices
{
    public class BookingOfferService : IBookingOfferService
    {
        private readonly IBaseRepository<BookingOffer> _bookingOfferRepo;
        private readonly IBaseRepository<RideRequest> _rideRequestRepo;
        private readonly IBaseRepository<Offer> _offerRepo;
        private readonly IBaseRepository<Booking> _bookingRepo;
        private readonly IBaseRepository<SharedRideOfferDetail> _sharedRideOfferDetailRepo;

        public BookingOfferService(
            IBaseRepository<BookingOffer> bookingOfferRepo,
            IBaseRepository<RideRequest> rideRequestRepo,
            IBaseRepository<Offer> offerRepo,
            IBaseRepository<Booking> bookingRepo,
            IBaseRepository<SharedRideOfferDetail> sharedRideOfferDetailRepo)
        {
            _bookingOfferRepo = bookingOfferRepo;
            _rideRequestRepo = rideRequestRepo;
            _offerRepo = offerRepo;
            _bookingRepo = bookingRepo;
            _sharedRideOfferDetailRepo = sharedRideOfferDetailRepo;
        }

        public async Task<BookingOfferResponseDTO> CreateAsync(Guid RideRequestId, Guid OfferId)
        {
            if (RideRequestId == Guid.Empty || OfferId == Guid.Empty)
                throw new AppException(400, "ID không hợp lệ");

            var rideRequest = await _rideRequestRepo.GetQueryable()
                .FirstOrDefaultAsync(r => r.Id == RideRequestId && r.DeleteAt == null);

            if (rideRequest == null) throw new AppException(404, "Không tìm thấy RideRequest");

            var offer = await _offerRepo.GetQueryable()
                .FirstOrDefaultAsync(o => o.Id == OfferId && o.DeleteAt == null);

            if (offer == null) throw new AppException(404, "Không tìm thấy Offer");

            // Kiểm tra xem đã tồn tại offer này chưa
            var existing = await _bookingOfferRepo.GetQueryable()
                .FirstOrDefaultAsync(b => b.RideRequestId == RideRequestId && b.OfferId == OfferId && b.DeleteAt == null);

            if (existing != null) throw new AppException(400, "Đã gửi đề nghị cho yêu cầu này rồi");

            var newBookingOffer = new BookingOffer
            {
                Id = Guid.NewGuid(),
                OfferId = OfferId,
                RideRequestId = RideRequestId,
                Status = BookingOfferStatus.Pending
            };

            await _bookingOfferRepo.AddAsync(newBookingOffer);
            await _bookingOfferRepo.SaveChangesAsync();

            return new BookingOfferResponseDTO
            {
                Id = newBookingOffer.Id,
                OfferId = newBookingOffer.OfferId,
                RideRequestId = newBookingOffer.RideRequestId,
                Status = newBookingOffer.Status,
                CreatedAt = newBookingOffer.CreatedAt
            };
        }

        public async Task<IEnumerable<BookingOfferResponseDTO>> GetByRideRequestAsync(Guid RideRequestId)
        {
            if (RideRequestId == Guid.Empty) throw new AppException(400, "ID không hợp lệ");

            var result = await _bookingOfferRepo.GetQueryable()
                .Where(b => b.RideRequestId == RideRequestId && b.DeleteAt == null)
                .Select(b => new BookingOfferResponseDTO
                {
                    Id = b.Id,
                    OfferId = b.OfferId,
                    RideRequestId = b.RideRequestId,
                    Status = b.Status,
                    BookingId = b.BookingId,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            return result;
        }

        public async Task AcceptAsync(Guid BookingOfferId)
        {
            if (BookingOfferId == Guid.Empty) throw new AppException(400, "ID không hợp lệ");

            var bookingOffer = await _bookingOfferRepo.GetQueryable()
                .Include(b => b.RideRequest)
                .Include(b => b.Offer)
                .ThenInclude(o => o.SharedRideDetail)
                .FirstOrDefaultAsync(b => b.Id == BookingOfferId && b.DeleteAt == null);

            if (bookingOffer == null) throw new AppException(404, "Không tìm thấy BookingOffer");

            if (bookingOffer.Status != BookingOfferStatus.Pending)
                throw new AppException(400, "Chỉ có thể chấp nhận đề nghị ở trạng thái chờ");

            var offer = bookingOffer.Offer;
            var rideRequest = bookingOffer.RideRequest;

            if (offer.SharedRideDetail != null)
            {
                if (offer.SharedRideDetail.AvailableSeats < rideRequest.SeatsRequired)
                {
                    throw new AppException(400, "Không đủ ghế trống cho yêu cầu này");
                }
                
                offer.SharedRideDetail.AvailableSeats -= rideRequest.SeatsRequired;
                _sharedRideOfferDetailRepo.Update(offer.SharedRideDetail);
            }

            var totalPrice = offer.BasePrice * rideRequest.SeatsRequired;

            var newBooking = new Booking
            {
                Id = Guid.NewGuid(),
                OfferId = bookingOffer.OfferId,
                CustomerId = rideRequest.CustomerId,
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Unpaid,
                Quantity = rideRequest.SeatsRequired,
                CreatedAt = DateTime.UtcNow,
                Items = new List<BookingItem>
                {
                    new BookingItem
                    {
                        Id = Guid.NewGuid(),
                        Price = totalPrice,
                        PickupLocationId = rideRequest.FromLocationId,
                        DropoffLocationId = rideRequest.ToLocationId
                    }
                }
            };

            await _bookingRepo.AddAsync(newBooking);

            // Cập nhật lại trạng thái
            bookingOffer.Status = BookingOfferStatus.Accepted;
            bookingOffer.BookingId = newBooking.Id;
            _bookingOfferRepo.Update(bookingOffer);

            rideRequest.Status = RideRequestStatus.Matched;
            _rideRequestRepo.Update(rideRequest);

            await _bookingOfferRepo.SaveChangesAsync();
        }

        public async Task RejectAsync(Guid BookingOfferId)
        {
            if (BookingOfferId == Guid.Empty) throw new AppException(400, "ID không hợp lệ");

            var bookingOffer = await _bookingOfferRepo.GetQueryable()
                .FirstOrDefaultAsync(b => b.Id == BookingOfferId && b.DeleteAt == null);

            if (bookingOffer == null) throw new AppException(404, "Không tìm thấy BookingOffer");

            if (bookingOffer.Status != BookingOfferStatus.Pending)
                throw new AppException(400, "Chỉ có thể từ chối đề nghị ở trạng thái chờ");

            bookingOffer.Status = BookingOfferStatus.Rejected;
            _bookingOfferRepo.Update(bookingOffer);

            await _bookingOfferRepo.SaveChangesAsync();
        }
    }
}
