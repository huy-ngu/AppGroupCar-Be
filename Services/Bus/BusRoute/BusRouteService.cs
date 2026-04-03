using AppGroupCar.Data;
using AppGroupCar.DTO.RequestDTO.Bus.RouteRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.RouteResponse;
using AppGroupCar.Entities.Bus;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Bus.BusRoute
{
    public class BusRouteService : IBusRouteService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BusRouteService(
            ApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // CREATE ROUTE
        public async Task<BusRouteResponse> CreateAsync(
        Guid companyId,
        CreateBusRouteRequest dto)
        {
            var route = new Entities.Bus.BusRoute
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                CompanyId = companyId
            };

            ValidateRoutePoints(dto.RoutePoints);

            foreach (var pointDto in dto.RoutePoints)
            {
                route.RoutePoints.Add(
                    new BusRoutePoint
                    {
                        Id = Guid.NewGuid(),
                        RouteId = route.Id,
                        LocationId = pointDto.LocationId,
                        Sequence = pointDto.Sequence,
                        PickupAllowed = pointDto.PickupAllowed,
                        DropoffAllowed = pointDto.DropoffAllowed
                    });
            }

            _context.BusRoutes.Add(route);

            await _context.SaveChangesAsync();

            return await GetByIdAsync(route.Id, companyId)
                ?? throw new Exception("Route not found");
        }

        // GET ALL ROUTES OF COMPANY

        public async Task<List<BusRouteResponse>> GetByCompanyAsync(
            Guid companyId)
        {
            var routes = await _context.BusRoutes
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.DeleteAt == null)
                .Include(x =>
                    x.RoutePoints.Where(p => p.DeleteAt == null))
                    .ThenInclude(x => x.Location)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return _mapper.Map<List<BusRouteResponse>>(routes);
        }

        // GET BY ID

        public async Task<BusRouteResponse?> GetByIdAsync(
            Guid id,
            Guid companyId)
        {
            var route = await _context.BusRoutes
                .Where(x =>
                    x.Id == id &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null)
                .Include(x =>
                    x.RoutePoints.Where(p => p.DeleteAt == null))
                    .ThenInclude(x => x.Location)
                .FirstOrDefaultAsync();

            if (route == null)
                return null;

            return _mapper.Map<BusRouteResponse>(route);
        }

        // UPDATE

        public async Task<BusRouteResponse> UpdateAsync(
            Guid id,
            Guid companyId,
            UpdateBusRouteRequest dto)
        {
            var route = await _context.BusRoutes
                .Include(x => x.RoutePoints)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null);

            if (route == null)
                throw new Exception("Route not found");

            ValidateRoutePoints(dto.RoutePoints);

            route.Name = dto.Name;
            route.LastUpdateAt = DateTime.UtcNow;

            var now = DateTime.UtcNow;
            var requestBySequence = dto.RoutePoints
                .ToDictionary(p => p.Sequence, p => p);

            foreach (var existingPoint in route.RoutePoints)
            {
                if (!requestBySequence.ContainsKey(existingPoint.Sequence))
                {
                    existingPoint.DeleteAt = now;
                    existingPoint.LastUpdateAt = now;
                }
            }

            foreach (var pointDto in dto.RoutePoints)
            {
                var existingPoint = route.RoutePoints
                    .FirstOrDefault(p => p.Sequence == pointDto.Sequence);

                if (existingPoint == null)
                {
                    route.RoutePoints.Add(new BusRoutePoint
                    {
                        Id = Guid.NewGuid(),
                        RouteId = route.Id,
                        LocationId = pointDto.LocationId,
                        Sequence = pointDto.Sequence,
                        PickupAllowed = pointDto.PickupAllowed,
                        DropoffAllowed = pointDto.DropoffAllowed
                    });

                    continue;
                }

                existingPoint.LocationId = pointDto.LocationId;
                existingPoint.PickupAllowed = pointDto.PickupAllowed;
                existingPoint.DropoffAllowed = pointDto.DropoffAllowed;
                existingPoint.DeleteAt = null;
                existingPoint.LastUpdateAt = now;
            }

            await _context.SaveChangesAsync();

            return await GetByIdAsync(route.Id, companyId)
                ?? throw new Exception("Route not found");
        }

        // DELETE

        public async Task DeleteAsync(
            Guid id,
            Guid companyId)
        {
            var route = await _context.BusRoutes
                .Include(x => x.RoutePoints)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null);

            if (route == null)
                throw new Exception("Route not found");

            var now = DateTime.UtcNow;
            route.DeleteAt = now;
            route.LastUpdateAt = now;

            foreach (var point in route.RoutePoints)
            {
                point.DeleteAt = now;
                point.LastUpdateAt = now;
            }

            await _context.SaveChangesAsync();
        }

        // VALIDATE ROUTE

        private static void ValidateRoutePoints(
            ICollection<CreateBusRoutePointRequest> points)
        {
            if (points.Count < 2)
                throw new Exception(
                    "Route must have at least 2 points");

            var sequences =
                points.Select(x => x.Sequence)
                      .OrderBy(x => x)
                      .ToList();

            if (sequences.Distinct().Count() != sequences.Count)
                throw new Exception("Sequence must be unique");

            for (int i = 0; i < sequences.Count; i++)
            {
                if (sequences[i] != i + 1)
                    throw new Exception(
                        "Sequence must start at 1 and be continuous");
            }
        }
    }
}
