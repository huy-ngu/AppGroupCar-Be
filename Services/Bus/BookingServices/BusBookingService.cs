using AppGroupCar.Data;
using AppGroupCar.DTO.RequestDTO.Bus.BookingRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.BookingResponse;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Bus.BookingServices
{
    public class BusBookingService : IBusBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BusBookingService(
            ApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BusBookingResponseDto> CreateAsync(Guid userId, CreateBusBookingRequest dto)
        {
            if (userId == Guid.Empty)
                throw new AppException(401, "Invalid user");

            var seatNumbers = NormalizeSeatNumbers(dto.SeatNumbers);

            if (!Enum.IsDefined(typeof(BusBookingStatus), dto.Status) ||
                dto.Status == BusBookingStatus.Cancelled ||
                dto.Status == BusBookingStatus.Expired)
            {
                throw new AppException(400, "Invalid booking status");
            }

            var showtime = await _context.BusShowtimes
                .FirstOrDefaultAsync(x => x.Id == dto.ShowtimeId && x.DeleteAt == null);

            if (showtime == null)
                throw new AppException(404, "Showtime not found");

            BusBooking? created = null;

            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var seats = await _context.Seats
                        .Where(x => x.ShowtimeId == dto.ShowtimeId && seatNumbers.Contains(x.SeatNumber) && x.DeleteAt == null)
                        .ToListAsync();

                    if (seats.Count != seatNumbers.Count)
                        throw new AppException(404, "Some seats not found");

                    var notAvailable = seats
                        .Where(x => x.Status != SeatStatus.Available)
                        .Select(x => x.SeatNumber)
                        .ToList();

                    if (notAvailable.Count > 0)
                        throw new AppException(409, "Seats are not available", notAvailable);

                    var now = DateTime.UtcNow;

                    created = new BusBooking
                    {
                        Id = Guid.NewGuid(),
                        ShowtimeId = dto.ShowtimeId,
                        UserId = userId,
                        TotalPrice = showtime.Price * seats.Count,
                        Status = dto.Status,
                        ExpireAt = null
                    };

                    _context.BusBookings.Add(created);

                    foreach (var seat in seats)
                    {
                        _context.BusBookingSeats.Add(new BusBookingSeat
                        {
                            Id = Guid.NewGuid(),
                            BookingId = created.Id,
                            SeatId = seat.SeatId,
                            SeatNumber = seat.SeatNumber,
                            TicketCode = GenerateTicketCode(),
                            IsCheckedIn = false
                        });

                        var oldStatus = seat.Status;
                        seat.Status = SeatStatus.Booked;
                        seat.LastUpdatedAt = now;

                        _context.SeatStatusHistories.Add(new SeatStatusHistory
                        {
                            Id = Guid.NewGuid(),
                            HistoryId = Guid.NewGuid(),
                            SeatId = seat.SeatId,
                            OldStatus = oldStatus,
                            NewStatus = SeatStatus.Booked,
                            Reason = "BOOKING",
                            ChangedBy = userId,
                            ChangedAt = now
                        });
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            if (created == null)
                throw new AppException(500, "Booking creation failed");

            return await GetByIdAsync(created.Id, userId);
        }

        public async Task<List<BusBookingResponseDto>> GetMyBookingsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new AppException(401, "Invalid user");

            var bookings = await _context.BusBookings
                .Where(x => x.UserId == userId && x.DeleteAt == null)
                .Include(x => x.Seats)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<BusBookingResponseDto>>(bookings);
        }

        public async Task<BusBookingResponseDto> GetByIdAsync(Guid bookingId, Guid userId)
        {
            if (bookingId == Guid.Empty)
                throw new AppException(400, "Invalid booking id");

            var booking = await _context.BusBookings
                .Where(x => x.Id == bookingId && x.UserId == userId && x.DeleteAt == null)
                .Include(x => x.Seats)
                .FirstOrDefaultAsync();

            if (booking == null)
                throw new AppException(404, "Booking not found");

            return _mapper.Map<BusBookingResponseDto>(booking);
        }

        private static List<string> NormalizeSeatNumbers(IEnumerable<string>? seatNumbers)
        {
            if (seatNumbers == null)
                throw new AppException(400, "Seat numbers are required");

            var normalized = seatNumbers
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (normalized.Count == 0)
                throw new AppException(400, "Seat numbers are required");

            var distinct = normalized
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (distinct.Count != normalized.Count)
                throw new AppException(400, "Duplicate seat numbers");

            return distinct;
        }

        private static string GenerateTicketCode()
        {
            return $"BUS-{Guid.NewGuid():N}";
        }
    }
}
