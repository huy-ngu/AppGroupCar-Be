using AppGroupCar.Data;
using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Bus.RouteRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.RouteResponse;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Exceptions;
using AppGroupCar.Services.RouteRules;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Bus.BusRoute
{
    public class BusRouteService : IBusRouteService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IRouteValidationService _routeValidationService;

        public BusRouteService(
            ApplicationDbContext context,
            IMapper mapper,
            IRouteValidationService routeValidationService)
        {
            _context = context;
            _mapper = mapper;
            _routeValidationService = routeValidationService;
        }

        public async Task<BusRouteResponse> CreateAsync(
            Guid companyId,
            CreateBusRouteRequest dto)
        {
            await ValidateRoutePointsAsync(dto.RoutePoints);

            var route = new Entities.Bus.BusRoute
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                EstimatedDurationMinutes = dto.EstimatedDurationMinutes,
                CompanyId = companyId
            };

            foreach (var pointDto in dto.RoutePoints.OrderBy(x => x.Sequence))
            {
                route.RoutePoints.Add(new BusRoutePoint
                {
                    Id = Guid.NewGuid(),
                    RouteId = route.Id,
                    LocationId = pointDto.LocationId,
                    Sequence = pointDto.Sequence,
                    StopType = pointDto.StopType
                });
            }

            _context.BusRoutes.Add(route);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(route.Id, companyId)
                ?? throw new AppException(404, "Route not found");
        }

        public async Task<List<BusRouteResponse>> GetByCompanyAsync(Guid companyId)
        {
            var routes = await _context.BusRoutes
                .Where(x => x.CompanyId == companyId && x.DeleteAt == null)
                .Include(x => x.RoutePoints.Where(p => p.DeleteAt == null))
                    .ThenInclude(x => x.Location)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return _mapper.Map<List<BusRouteResponse>>(routes);
        }

        public async Task<BusRouteResponse?> GetByIdAsync(Guid id, Guid companyId)
        {
            var route = await _context.BusRoutes
                .Where(x => x.Id == id && x.CompanyId == companyId && x.DeleteAt == null)
                .Include(x => x.RoutePoints.Where(p => p.DeleteAt == null))
                    .ThenInclude(x => x.Location)
                .FirstOrDefaultAsync();

            return route == null
                ? null
                : _mapper.Map<BusRouteResponse>(route);
        }

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
            {
                throw new AppException(404, "Route not found");
            }

            await ValidateRoutePointsAsync(dto.RoutePoints);

            route.Name = dto.Name;
            route.EstimatedDurationMinutes = dto.EstimatedDurationMinutes ?? route.EstimatedDurationMinutes;
            route.LastUpdateAt = DateTime.UtcNow;

            var now = DateTime.UtcNow;
            var requestBySequence = dto.RoutePoints.ToDictionary(p => p.Sequence, p => p);

            foreach (var existingPoint in route.RoutePoints)
            {
                if (!requestBySequence.ContainsKey(existingPoint.Sequence))
                {
                    existingPoint.DeleteAt = now;
                    existingPoint.LastUpdateAt = now;
                }
            }

            foreach (var pointDto in dto.RoutePoints.OrderBy(x => x.Sequence))
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
                        StopType = pointDto.StopType
                    });

                    continue;
                }

                existingPoint.LocationId = pointDto.LocationId;
                existingPoint.StopType = pointDto.StopType;
                existingPoint.DeleteAt = null;
                existingPoint.LastUpdateAt = now;
            }

            await _context.SaveChangesAsync();

            return await GetByIdAsync(route.Id, companyId)
                ?? throw new AppException(404, "Route not found");
        }

        public async Task DeleteAsync(Guid id, Guid companyId)
        {
            var route = await _context.BusRoutes
                .Include(x => x.RoutePoints)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null);

            if (route == null)
            {
                throw new AppException(404, "Route not found");
            }

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

        public async Task<bool> ExistsForCompanyAsync(Guid routeId, Guid companyId)
        {
            return await _context.BusRoutes
                .AnyAsync(x =>
                    x.Id == routeId &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null);
        }

        public async Task<PagedResult<BusRouteResponse>> GetAsync(
            Guid companyId,
            BusRouteParameters param)
        {
            var query = _context.BusRoutes
                .Where(x => x.CompanyId == companyId && x.DeleteAt == null)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(param.Search))
            {
                var searchTerm = param.Search.Trim().ToLower();
                query = query.Where(x => x.Name != null && x.Name.ToLower().Contains(searchTerm));
            }

            query = query.OrderBy(x => x.Name);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((param.PageNumber - 1) * param.PageSize)
                .Take(param.PageSize)
                .ProjectTo<BusRouteResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<BusRouteResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize
            };
        }

        private async Task ValidateRoutePointsAsync(
            ICollection<CreateBusRoutePointRequest> points)
        {
            var routeStops = points
                .Select(x => new RouteStopInput(x.LocationId, x.Sequence, x.StopType))
                .ToList();

            await _routeValidationService.ValidateRouteForCreationAsync(
                new RouteValidationRequest(RouteTransportMode.Bus, routeStops));
        }
    }
}
