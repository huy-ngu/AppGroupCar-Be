using AppGroupCar.Data;
using AppGroupCar.DTO.RequestDTO.Bus.SeatStatusHistoryRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.SeatStatusHistoryResponse;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Bus.SeatStatusHistoryServices
{
    public class SeatStatusHistoryService : ISeatStatusHistoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SeatStatusHistoryService(
            ApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<SeatStatusHistoryResponseDto>> ChangeStatusAsync(Guid userId, ChangeSeatStatusRequest dto)
        {
            if (userId == Guid.Empty)
                throw new AppException(401, "Invalid user");

            var seatNumbers = NormalizeSeatNumbers(dto.SeatNumbers);

            var seats = await _context.Seats
                .Where(x => x.ShowtimeId == dto.ShowtimeId && seatNumbers.Contains(x.SeatNumber) && x.DeleteAt == null)
                .ToListAsync();

            if (seats.Count != seatNumbers.Count)
                throw new AppException(404, "Some seats not found");

            var sameStatus = seats
                .Where(x => x.Status == dto.NewStatus)
                .Select(x => x.SeatNumber)
                .ToList();

            if (sameStatus.Count > 0)
                throw new AppException(409, "Seats already in requested status", sameStatus);

            var now = DateTime.UtcNow;
            var reason = dto.Reason?.Trim();

            var histories = seats.Select(seat => new SeatStatusHistory
            {
                Id = Guid.NewGuid(),
                HistoryId = Guid.NewGuid(),
                SeatId = seat.SeatId,
                OldStatus = seat.Status,
                NewStatus = dto.NewStatus,
                Reason = reason,
                ChangedBy = userId,
                ChangedAt = now,
                Seat = seat
            }).ToList();

            foreach (var seat in seats)
            {
                seat.Status = dto.NewStatus;
                seat.LastUpdatedAt = now;
            }

            _context.SeatStatusHistories.AddRange(histories);
            await _context.SaveChangesAsync();

            return _mapper.Map<List<SeatStatusHistoryResponseDto>>(histories);
        }

        public async Task<List<SeatStatusHistoryResponseDto>> GetBySeatAsync(Guid seatId)
        {
            if (seatId == Guid.Empty)
                throw new AppException(400, "Invalid seat id");

            var histories = await _context.SeatStatusHistories
                .Where(x => x.SeatId == seatId)
                .Include(x => x.Seat)
                .OrderByDescending(x => x.ChangedAt)
                .ToListAsync();

            return _mapper.Map<List<SeatStatusHistoryResponseDto>>(histories);
        }

        public async Task<List<SeatStatusHistoryResponseDto>> GetByShowtimeAsync(Guid showtimeId)
        {
            if (showtimeId == Guid.Empty)
                throw new AppException(400, "Invalid showtime id");

            var histories = await _context.SeatStatusHistories
                .Where(x => x.Seat.ShowtimeId == showtimeId)
                .Include(x => x.Seat)
                .OrderByDescending(x => x.ChangedAt)
                .ToListAsync();

            return _mapper.Map<List<SeatStatusHistoryResponseDto>>(histories);
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
    }
}
