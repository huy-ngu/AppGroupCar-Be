using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO.Market.Offer;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Services.RouteRules;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Market.OfferServices.OfferShipmentServices
{
    public class OfferShipmentService : IOfferShipmentSerivce
    {
        private readonly IBaseRepository<Offer> _offerRepo;
        private readonly IBaseRepository<CargoOfferDetail> _cargoRepo;
        private readonly IBaseRepository<OfferRoutePoint> _routePointRepo;
        private readonly IBaseRepository<Vehicle> _vehicleRepo;
        private readonly IBaseRepository<Location> _locationRepo;
        private readonly IMapper _mapper;
        private readonly IRouteValidationService _routeValidationService;
        private readonly IRouteRulePolicy _routeRulePolicy;

        public OfferShipmentService(
            IBaseRepository<Offer> offerRepo,
            IBaseRepository<CargoOfferDetail> cargoRepo,
            IBaseRepository<OfferRoutePoint> routePointRepo,
            IBaseRepository<Vehicle> vehicleRepo,
            IBaseRepository<Location> locationRepo,
            IMapper mapper,
            IRouteValidationService routeValidationService,
            IRouteRulePolicy routeRulePolicy)
        {
            _offerRepo = offerRepo;
            _cargoRepo = cargoRepo;
            _routePointRepo = routePointRepo;
            _vehicleRepo = vehicleRepo;
            _locationRepo = locationRepo;
            _mapper = mapper;
            _routeValidationService = routeValidationService;
            _routeRulePolicy = routeRulePolicy;
        }

        public async Task<GetOfferDTO> CreateWithCargoAsync(Guid driverId, CreateOfferShipmentRequestDTO request)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId);

            if (vehicle == null || vehicle.DeleteAt != null || vehicle.DriverId != driverId)
            {
                throw new AppException(400, "Vehicle không tồn tại hoặc không thuộc về bạn");
            }

            if (request.DepartureTime < DateTime.UtcNow)
            {
                throw new AppException(400, "Thời gian không hợp lệ");
            }

            if (request.OfferRoutePoints.Count < 2)
            {
                throw new AppException(400, "Offer phải có ít nhất 2 route stop");
            }

            var recentOffer = await _offerRepo.GetQueryable()
                .AsNoTracking()
                .Where(o =>
                    o.DriverId == driverId &&
                    o.Status == OfferStatus.Active &&
                    o.DeleteAt == null)
                .OrderByDescending(o => o.DepartureTime)
                .FirstOrDefaultAsync();

            if (recentOffer != null)
            {
                throw new AppException(400, "Bạn đang có offer hoạt động. Vui lòng hoàn thành offer cũ trước khi tạo mới");
            }

            var routeStops = request.OfferRoutePoints
                .Select((item, index) => new RouteStopInput(
                    item.LocationId,
                    index + 1,
                    item.StopType))
                .ToList();

            await _routeValidationService.ValidateRouteForCreationAsync(
                new RouteValidationRequest(RouteTransportMode.Truck, routeStops));

            var offer = _mapper.Map<Offer>(request);
            offer.Id = Guid.NewGuid();
            offer.DriverId = driverId;
            offer.Status = OfferStatus.Active;
            offer.ServiceType = ServiceType.Truck;
            offer.EstimatedDurationMinutes = request.EstimatedDurationMinutes;

            var routePoints = request.OfferRoutePoints
                .Select((item, index) => new OfferRoutePoint
                {
                    Id = Guid.NewGuid(),
                    OfferId = offer.Id,
                    Sequence = index + 1,
                    LocationId = item.LocationId,
                    StopType = item.StopType
                })
                .ToList();

            var cargo = new CargoOfferDetail
            {
                Id = offer.Id,
                MaxVolume = request.Cargo.MaxVolume,
                MaxWeight = request.Cargo.MaxWeight,
                AcceptFragile = request.Cargo.AcceptFragile
            };

            await _routePointRepo.AddRangeAsync(routePoints);
            await _offerRepo.AddAsync(offer);
            await _cargoRepo.AddAsync(cargo);
            await _offerRepo.SaveChangesAsync();

            return _mapper.Map<GetOfferDTO>(offer);
        }

        public async Task<PagedResult<GetOfferShipmentDTO>> GetOfferShipmentAsync(GetOfferForUserRequestDTO request)
        {
            var pickupStopTypes = _routeRulePolicy.PickupStopTypes.ToList();
            var dropoffStopTypes = _routeRulePolicy.DropoffStopTypes.ToList();

            var query = _offerRepo.GetQueryable()
                .Include(o => o.CargoDetail)
                .Include(o => o.RoutePoints)
                .Include(o => o.Driver)
                .Include(o => o.Vehicle)
                .Where(o => o.Status == OfferStatus.Active && o.ServiceType == ServiceType.Truck && o.DeleteAt == null);

            if (request.startTime.HasValue)
            {
                query = query.Where(o => o.DepartureTime >= request.startTime.Value);
            }

            if (request.endTime.HasValue)
            {
                query = query.Where(o => o.DepartureTime <= request.endTime.Value);
            }

            if (request.startPrice.HasValue)
            {
                query = query.Where(o => o.BasePrice >= request.startPrice.Value);
            }

            if (request.endPrice.HasValue)
            {
                query = query.Where(o => o.BasePrice <= request.endPrice.Value);
            }

            if (request.PickupLocationId.HasValue && request.DropoffLocationId.HasValue)
            {
                var pickupLocationIds = await GetDescendantRouteLocationIdsAsync(request.PickupLocationId.Value);
                var dropoffLocationIds = await GetDescendantRouteLocationIdsAsync(request.DropoffLocationId.Value);

                query = query.Where(o => o.RoutePoints.Any(pickup =>
                    pickupLocationIds.Contains(pickup.LocationId) &&
                    pickupStopTypes.Contains(pickup.StopType) &&
                    o.RoutePoints.Any(dropoff =>
                        dropoffLocationIds.Contains(dropoff.LocationId) &&
                        dropoffStopTypes.Contains(dropoff.StopType) &&
                        pickup.Sequence < dropoff.Sequence)));
            }
            else if (request.PickupLocationId.HasValue)
            {
                var pickupLocationIds = await GetDescendantRouteLocationIdsAsync(request.PickupLocationId.Value);

                query = query.Where(o => o.RoutePoints.Any(rp =>
                    pickupLocationIds.Contains(rp.LocationId) &&
                    pickupStopTypes.Contains(rp.StopType)));
            }
            else if (request.DropoffLocationId.HasValue)
            {
                var dropoffLocationIds = await GetDescendantRouteLocationIdsAsync(request.DropoffLocationId.Value);

                query = query.Where(o => o.RoutePoints.Any(rp =>
                    dropoffLocationIds.Contains(rp.LocationId) &&
                    dropoffStopTypes.Contains(rp.StopType)));
            }

            var totalRecords = await query.CountAsync();
            
            var offers = await query
                .OrderBy(o => o.DepartureTime)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync();

            var data = new List<GetOfferShipmentDTO>();
            foreach (var offer in offers)
            {
                var dto = _mapper.Map<GetOfferShipmentDTO>(offer);

                var startPoint = offer.RoutePoints.FirstOrDefault(rp => rp.StopType == RouteStopType.Start);
                var endPoint = offer.RoutePoints.FirstOrDefault(rp => rp.StopType == RouteStopType.End);

                if (startPoint != null)
                {
                    var startLocation = await _locationRepo.GetByIdAsync(startPoint.LocationId);
                    dto.StartPoint = new LocationDetailDTO
                    {
                        LocationId = startPoint.LocationId,
                        LocationName = startLocation?.Name ?? ""
                    };
                }

                if (endPoint != null)
                {
                    var endLocation = await _locationRepo.GetByIdAsync(endPoint.LocationId);
                    dto.EndPoint = new LocationDetailDTO
                    {
                        LocationId = endPoint.LocationId,
                        LocationName = endLocation?.Name ?? ""
                    };
                }

                data.Add(dto);
            }

            return new PagedResult<GetOfferShipmentDTO>
            {
                Items = data,
                TotalCount = totalRecords,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        private async Task<List<Guid>> GetDescendantRouteLocationIdsAsync(Guid locationId)
        {
            var allowedTypeCodes = _routeRulePolicy.RouteCreationLocationTypes
                .Select(x => (int)x)
                .ToList();

            var location = await _locationRepo.GetQueryable()
                .AsNoTracking()
                .Where(x => x.Id == locationId && x.DeleteAt == null && x.IsActive)
                .FirstOrDefaultAsync();

            if (location == null)
            {
                return new List<Guid>();
            }

            if (location.LocationType is LocationType.Province or LocationType.District)
            {
                var matchedIds = new HashSet<Guid>();
                var visitedIds = new HashSet<Guid> { locationId };
                var frontier = new List<Guid> { locationId };

                while (frontier.Count > 0)
                {
                    var children = await _locationRepo.GetQueryable()
                        .AsNoTracking()
                        .Where(x =>
                            x.DeleteAt == null &&
                            x.IsActive &&
                            x.ParentLocationId.HasValue &&
                            frontier.Contains(x.ParentLocationId.Value))
                        .Select(x => new { x.Id, x.LocationType })
                        .ToListAsync();

                    var nextFrontier = new List<Guid>();

                    foreach (var child in children)
                    {
                        if (!visitedIds.Add(child.Id))
                        {
                            continue;
                        }

                        nextFrontier.Add(child.Id);

                        if (allowedTypeCodes.Contains((int)child.LocationType))
                        {
                            matchedIds.Add(child.Id);
                        }
                    }

                    frontier = nextFrontier;
                }

                if (allowedTypeCodes.Contains((int)location.LocationType))
                {
                    matchedIds.Add(locationId);
                }

                return matchedIds.ToList();
            }

            return new List<Guid> { locationId };
        }
    }
}
