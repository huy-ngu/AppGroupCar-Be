using AppGroupCar.Data;
using AppGroupCar.DTO.ResponseDTO.Bus.SeatResponse;
using AppGroupCar.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
            await EnsureShowtimeExistsAsync(showtimeId);
            return await LoadSeatMapAsync(showtimeId);
        }

        public async Task<SeatLayoutSnapshotDto> GetSeatLayoutAsync(Guid showtimeId)
        {
            await EnsureShowtimeExistsAsync(showtimeId);
            return await LoadSeatLayoutAsync(showtimeId);
        }

        public async Task<ShowtimeSeatMapResponseDto> GetSeatMapWithLayoutAsync(Guid showtimeId)
        {
            await EnsureShowtimeExistsAsync(showtimeId);

            var seatLayout = await LoadSeatLayoutAsync(showtimeId);
            var seats = await LoadSeatMapAsync(showtimeId);

            return new ShowtimeSeatMapResponseDto
            {
                ShowtimeId = showtimeId,
                SeatLayout = seatLayout,
                Seats = seats
            };
        }

        private async Task EnsureShowtimeExistsAsync(Guid showtimeId)
        {
            var showtimeExists = await _context.BusShowtimes
                .AnyAsync(x => x.Id == showtimeId && x.DeleteAt == null);

            if (!showtimeExists)
                throw new AppException(404, "Showtime not found");
        }

        private async Task<List<ShowtimeSeatResponseDto>> LoadSeatMapAsync(Guid showtimeId)
        {
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

        private async Task<SeatLayoutSnapshotDto> LoadSeatLayoutAsync(Guid showtimeId)
        {
            var companyVehicleId = await _context.BusShowtimes
                .Where(x => x.Id == showtimeId && x.DeleteAt == null)
                .Select(x => (Guid?)x.CompanyVehicleId)
                .FirstOrDefaultAsync();

            if (!companyVehicleId.HasValue)
                throw new AppException(404, "Showtime not found");

            var seatLayout = await _context.CompanyVehicles
                .Where(x => x.CompanyVehicleId == companyVehicleId && x.DeleteAt == null)
                .Join(
                    _context.VehicleSeatLayouts
                        .Where(x => x.DeleteAt == null && x.IsActive),
                    companyVehicle => companyVehicle.SeatLayoutId,
                    layout => layout.Id,
                    (_, layout) => new
                    {
                        layout.Id,
                        layout.SeatLayoutName,
                        layout.SeatCapacity,
                        layout.LayoutJson,
                        layout.Description
                    })
                .FirstOrDefaultAsync();

            if (seatLayout == null)
                throw new AppException(404, "Seat layout not found");

            return new SeatLayoutSnapshotDto
            {
                Id = seatLayout.Id,
                SeatLayoutName = seatLayout.SeatLayoutName,
                SeatCapacity = seatLayout.SeatCapacity,
                LayoutJson = JsonSerializer.Deserialize<JsonElement>(seatLayout.LayoutJson, (JsonSerializerOptions)null),
                Description = seatLayout.Description
            };
        }
    }
}
