using AppGroupCar.Data;
using AppGroupCar.DTO.RequestDTO.Bus.ShowtimeRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.ShowtimeResponse;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Exceptions;
using AppGroupCar.Services.Bus.BusRoute;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AppGroupCar.Services.Bus.ShowtimeServices
{
    public class BusShowtimeService : IBusShowtimeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IBusRouteService _busRouteService;
        private readonly BusShowtimeSeatCreator _seatCreator;

        public BusShowtimeService(
            ApplicationDbContext context,
            IMapper mapper,
            IBusRouteService busRouteService)
        {
            _context = context;
            _mapper = mapper;
            _busRouteService = busRouteService;
            _seatCreator = new BusShowtimeSeatCreator(_context);
        }

        public async Task<BusShowtimeResponse> CreateAsync(
            Guid companyId,
            Guid userId,
            CreateBusShowtimeRequest dto)
        {
            await ValidateRouteAsync(companyId, dto.RouteId);

            var duplicate = await _context.BusShowtimes.AnyAsync(x =>
                x.CompanyVehicleId == dto.CompanyVehicleId &&
                x.DepartureDate == dto.DepartureDate &&
                x.DepartureTime == dto.DepartureTime &&
                x.DeleteAt == null);

            if (duplicate)
                throw new AppException(409, "A showtime already exists at this date and time for the selected vehicle");

            // 2) Lấy LayoutJson theo companyId + companyVehicleId
            var seatLayoutJson = await GetLayoutJsonAsync(companyId, dto.CompanyVehicleId);

            var showtime = BuildShowtimeEntity(companyId, userId, dto);

            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1) Lưu BusShowtime
                    await SaveShowtimeAsync(showtime);

                    // 3) Sinh danh sách Seat từ LayoutJson
                    var seatNumbers = _seatCreator.ExtractSeatNumbers(seatLayoutJson);
                    var seats = _seatCreator.BuildSeatEntities(showtime.Id, seatNumbers);

                    // 4) Insert toàn bộ Seat
                    await _seatCreator.SaveSeatsAsync(seats);

                    await AddShowtimeLogAsync(showtime.Id, userId, "CREATE", null, showtime.Status);

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            return await GetResponseByIdAsync(showtime.Id, companyId)
                ?? throw new AppException(404, "Showtime not found");
        }

        public async Task<List<BusShowtimeResponse>> GetByCompanyAsync(Guid companyId)
        {
            var entities = await _context.BusShowtimes
                .Where(x => x.CompanyId == companyId && x.DeleteAt == null)
                .Include(x => x.CompanyVehicle)
                .Include(x => x.Seats)
                .OrderBy(x => x.DepartureDate)
                .ThenBy(x => x.DepartureTime)
                .ToListAsync();

            return _mapper.Map<List<BusShowtimeResponse>>(entities);
        }

        public async Task<List<BusShowtimeResponse>> FilterAsync(
            Guid companyId,
            DateOnly departureDate,
            string? plateNumber,
            TimeOnly? departureTime)
        {
            var query = _context.BusShowtimes
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.DepartureDate == departureDate &&
                    x.DeleteAt == null)
                .Include(x => x.CompanyVehicle)
                .Include(x => x.Seats)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(plateNumber))
            {
                var normalizedPlate = plateNumber.Trim().ToLower();
                query = query.Where(x => x.CompanyVehicle.PlateNumber.ToLower() == normalizedPlate);
            }

            if (departureTime.HasValue)
                query = query.Where(x => x.DepartureTime == departureTime.Value);

            var entities = await query
                .OrderBy(x => x.DepartureTime)
                .ToListAsync();

            return _mapper.Map<List<BusShowtimeResponse>>(entities);
        }

        public async Task<BusShowtimeResponse> UpdateAsync(
            Guid id,
            Guid companyId,
            UpdateBusShowtimeRequest dto)
        {
            var entity = await _context.BusShowtimes.FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId &&
                x.DeleteAt == null);

            if (entity == null)
                throw new AppException(404, "Showtime not found");

            var companyVehicleId = dto.CompanyVehicleId ?? entity.CompanyVehicleId;
            var routeId = dto.RouteId ?? entity.RouteId;
            var departureDate = dto.DepartureDate ?? entity.DepartureDate;
            var departureTime = dto.DepartureTime ?? entity.DepartureTime;

            if (dto.RouteId.HasValue)
                await ValidateRouteAsync(companyId, routeId);

            if (dto.CompanyVehicleId.HasValue)
                await GetLayoutJsonAsync(companyId, companyVehicleId);

            var duplicate = await _context.BusShowtimes.AnyAsync(x =>
                x.Id != id &&
                x.CompanyVehicleId == companyVehicleId &&
                x.DepartureDate == departureDate &&
                x.DepartureTime == departureTime &&
                x.DeleteAt == null);

            if (duplicate)
                throw new AppException(409, "A showtime already exists at this date and time for the selected vehicle");

            if (dto.CompanyVehicleId.HasValue)
                entity.CompanyVehicleId = companyVehicleId;

            if (dto.RouteId.HasValue)
                entity.RouteId = routeId;

            if (dto.DepartureDate.HasValue)
                entity.DepartureDate = departureDate;

            if (dto.DepartureTime.HasValue)
                entity.DepartureTime = departureTime;

            if (dto.Price.HasValue)
                entity.Price = dto.Price.Value;

            entity.LastUpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetResponseByIdAsync(entity.Id, companyId)
                ?? throw new AppException(404, "Showtime not found");
        }

        public async Task DeleteAsync(
            Guid id,
            Guid companyId,
            Guid userId)
        {
            var entity = await _context.BusShowtimes
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null);

            if (entity == null)
                throw new AppException(404, "Showtime not found");

            var now = DateTime.UtcNow;
            entity.DeleteAt = now;
            entity.LastUpdateAt = now;

            await _context.SaveChangesAsync();
            await AddShowtimeLogAsync(entity.Id, userId, "DELETE", entity.Status, entity.Status);
        }

        public async Task<BusShowtimeResponse> ChangeStatusAsync(
            Guid id,
            Guid companyId,
            Guid userId,
            ChangeBusShowtimeStatusRequest dto)
        {
            var entity = await _context.BusShowtimes.FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId &&
                x.DeleteAt == null);

            if (entity == null)
                throw new AppException(404, "Showtime not found");

            var oldStatus = entity.Status;
            entity.Status = dto.Status;
            entity.LastUpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await AddShowtimeLogAsync(entity.Id, userId, "CHANGE_STATUS", oldStatus, entity.Status);

            return await GetResponseByIdAsync(entity.Id, companyId)
                ?? throw new AppException(404, "Showtime not found");
        }

        private async Task<BusShowtimeResponse?> GetResponseByIdAsync(Guid id, Guid companyId)
        {
            var entity = await _context.BusShowtimes
                .Where(x =>
                    x.Id == id &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null)
                .Include(x => x.CompanyVehicle)
                .Include(x => x.Seats)
                .FirstOrDefaultAsync();

            return entity == null
                ? null
                : _mapper.Map<BusShowtimeResponse>(entity);
        }

        private async Task ValidateRouteAsync(
            Guid companyId,
            Guid routeId)
        {
            var routeExists = await _busRouteService.ExistsForCompanyAsync(routeId, companyId);
            if (!routeExists)
                throw new AppException(404, "Route not found for this company");
        }

        private async Task<string> GetLayoutJsonAsync(
            Guid companyId,
            Guid companyVehicleId)
        {
            var seatLayoutJson = await _context.CompanyVehicles
                .Where(x =>
                    x.CompanyVehicleId == companyVehicleId &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null)
                .Join(
                    _context.VehicleSeatLayouts.Where(x => x.DeleteAt == null),
                    companyVehicle => companyVehicle.SeatLayoutId,
                    seatLayout => seatLayout.Id,
                    (_, seatLayout) => seatLayout.LayoutJson)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(seatLayoutJson))
                throw new AppException(404, "Company vehicle or seat layout not found");

            return seatLayoutJson;
        }

        private static BusShowtime BuildShowtimeEntity(
            Guid companyId,
            Guid userId,
            CreateBusShowtimeRequest dto)
        {
            return new BusShowtime
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                CompanyVehicleId = dto.CompanyVehicleId,
                RouteId = dto.RouteId,
                DepartureDate = dto.DepartureDate,
                DepartureTime = dto.DepartureTime,
                Price = dto.Price,
                Status = BusShowtimeStatus.Scheduled,
                CreatedBy = userId
            };
        }

        private async Task SaveShowtimeAsync(BusShowtime showtime)
        {
            _context.BusShowtimes.Add(showtime);
            await _context.SaveChangesAsync();
        }

        private async Task AddShowtimeLogAsync(
            Guid showtimeId,
            Guid userId,
            string action,
            BusShowtimeStatus? oldStatus,
            BusShowtimeStatus? newStatus)
        {
            _context.ShowtimeLogs.Add(new ShowtimeLog
            {
                Id = Guid.NewGuid(),
                ShowtimeId = showtimeId,
                Action = action,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                CreatedBy = userId
            });

            await _context.SaveChangesAsync();
        }
    }
}
