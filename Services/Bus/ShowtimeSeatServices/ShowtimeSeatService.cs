using AppGroupCar.Data;
using AppGroupCar.DTO.ResponseDTO.Bus.SeatResponse;
using AppGroupCar.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Bus.ShowtimeSeatServices
{
    public class ShowtimeSeatService : IShowtimeSeatService
    {
        private readonly ApplicationDbContext _context;

        public ShowtimeSeatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ShowtimeSeatResponseDto>> GetSeatMapAsync(Guid showtimeId)
        {
            var showtimeExists = await _context.BusShowtimes
                .AnyAsync(x => x.Id == showtimeId && x.DeleteAt == null);

            if (!showtimeExists)
                throw new AppException(404, "Showtime not found");

            return await _context.Seats
                .Where(x => x.ShowtimeId == showtimeId && x.DeleteAt == null)
                .OrderBy(x => x.SeatNumber)
                .Select(x => new ShowtimeSeatResponseDto
                {
                    SeatNumber = x.SeatNumber,
                    Status = x.Status.ToString()
                })
                .ToListAsync();
        }
    }
}
