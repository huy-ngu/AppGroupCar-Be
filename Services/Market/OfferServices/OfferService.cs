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

namespace AppGroupCar.Services.Market.OfferServices
{
    public class OfferService : IOfferService
    {
        private readonly IBaseRepository<Offer> _repo;
        private readonly IBaseRepository<Vehicle> _vehicleRepo;
        private readonly IBaseRepository<SharedRideOfferDetail> _repoOfferDetail;
        private readonly IBaseRepository<OfferRoutePoint> _repoOfferRoutePoint;
        private readonly IBaseRepository<Booking> _bookingRepo;
        private readonly IBaseRepository<Location> _locationRepo;
        private readonly IMapper _mapper;
        private readonly IRouteValidationService _routeValidationService;
        private readonly IRouteRulePolicy _routeRulePolicy;

        public OfferService(
            IBaseRepository<Offer> repo,
            IBaseRepository<Vehicle> vehicleRepo,
            IBaseRepository<SharedRideOfferDetail> repoOfferDetail,
            IBaseRepository<OfferRoutePoint> repoOfferRoutePoint,
            IBaseRepository<Booking> bookingRepo,
            IBaseRepository<Location> locationRepo,
            IMapper mapper,
            IRouteValidationService routeValidationService,
            IRouteRulePolicy routeRulePolicy)
        {
            _repo = repo;
            _vehicleRepo = vehicleRepo;
            _repoOfferDetail = repoOfferDetail;
            _repoOfferRoutePoint = repoOfferRoutePoint;
            _bookingRepo = bookingRepo;
            _locationRepo = locationRepo;
            _mapper = mapper;
            _routeValidationService = routeValidationService;
            _routeRulePolicy = routeRulePolicy;
        }

        public async Task DeleteAsync(Guid driverId, Guid id)
        {
            var offer = await _repo.GetByIdAsync(id);
            if (offer == null || offer.DeleteAt != null || offer.DriverId != driverId)
            {
                throw new AppException(404, "Không tìm thấy bản ghi hoặc bạn không có quyền xóa");
            }

            _repo.Delete(offer);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<GetOfferDTO>> GetAllAsync()
        {
            var offers = await GetOfferWithRelationsQuery()
                .Where(x => x.DeleteAt == null)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetOfferDTO>>(offers);
        }

        public async Task<IEnumerable<GetOfferDTO>> GetMyOffersAsync(Guid driverId)
        {
            var offers = await _repo.GetQueryable()
                .Where(o => o.DriverId == driverId && o.DeleteAt == null)
                .Include(o => o.Driver)
                .Include(o => o.Vehicle)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetOfferDTO>>(offers);
        }

        public async Task<GetOfferDTO> GetByIdAsync(Guid id)
        {
            var offer = await GetOfferWithRelationsQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (offer == null || offer.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }

            return _mapper.Map<GetOfferDTO>(offer);
        }

        public async Task<GetOfferDTO> UpdateAsync(Guid driverId, UpdateOfferRequestDTO request)
        {
            var offer = await GetOfferWithRelationsQuery()
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (offer == null || offer.DeleteAt != null || offer.DriverId != driverId)
            {
                throw new AppException(404, "Không tìm thấy bản ghi hoặc bạn không có quyền cập nhật");
            }

            if (request.VehicleId.HasValue)
            {
                var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId.Value);
                if (vehicle == null || vehicle.DriverId != driverId || vehicle.DeleteAt != null)
                {
                    throw new AppException(400, "Vehicle không tồn tại hoặc không thuộc về bạn");
                }

                offer.VehicleId = request.VehicleId.Value;
                offer.Vehicle = vehicle;
            }

            _mapper.Map(request, offer);

            _repo.Update(offer);
            await _repo.SaveChangesAsync();

            return _mapper.Map<GetOfferDTO>(offer);
        }

        public async Task<GetOfferDTO> CreateWithDetailAsync(Guid driverId, CreateOfferSharedRidelRequestDTO request)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId);
            if (vehicle == null || vehicle.DeleteAt != null || vehicle.DriverId != driverId)
            {
                throw new AppException(404, "Vehicle không tồn tại hoặc không thuộc về bạn");
            }
            if(vehicle.Status != VehicleStatus.Active)
            {
                throw new AppException(400, "Phương tiện không đủ điều kiện, Vui lòng chọn phương tiện đang hoạt động");
            }

            if (request.DepartureTime < DateTime.UtcNow)
            {
                throw new AppException(400, "Thời gian không hợp lệ");
            }

            if (request.OfferRoutePoints.Count < 2)
            {
                throw new AppException(400, "Offer phải có ít nhất 2 route stop");
            }

            var recentOffer = await _repo.GetQueryable()
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
                new RouteValidationRequest(RouteTransportMode.SharedRide, routeStops));

            var offer = _mapper.Map<Offer>(request);
            offer.Id = Guid.NewGuid();
            offer.DriverId = driverId;
            offer.ServiceType = ServiceType.Shared;
            offer.Status = OfferStatus.Active;
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

            var offerDetail = new SharedRideOfferDetail
            {
                Id = offer.Id,
                AvailableSeats = vehicle.SeatCapacity,
                TotalSeats = vehicle.SeatCapacity
            };

            await _repoOfferRoutePoint.AddRangeAsync(routePoints);
            await _repo.AddAsync(offer);
            await _repoOfferDetail.AddAsync(offerDetail);
            await _repo.SaveChangesAsync();

            return _mapper.Map<GetOfferDTO>(offer);
        }

        public async Task<PagedResult<GetOfferSharedRideDTO>> GetOfferSharedRideAsync(GetOfferForUserRequestDTO request)
        {
            var pickupStopTypes = _routeRulePolicy.PickupStopTypes.ToList();
            var dropoffStopTypes = _routeRulePolicy.DropoffStopTypes.ToList();

            var query = _repo.GetQueryable()
                .Include(o => o.SharedRideDetail)
                .Include(o => o.RoutePoints)
                .Include(o => o.Driver)
                .Include(o => o.Vehicle)
                .Where(o => o.Status == OfferStatus.Active && o.ServiceType == ServiceType.Shared && o.DeleteAt == null);

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
                var pickupId = request.PickupLocationId.Value;
                var dropoffId = request.DropoffLocationId.Value;

                query = query.Where(o => o.RoutePoints.Any(pickup =>
                    pickup.LocationId == pickupId &&
                    pickupStopTypes.Contains(pickup.StopType) &&
                    o.RoutePoints.Any(dropoff =>
                        dropoff.LocationId == dropoffId &&
                        dropoffStopTypes.Contains(dropoff.StopType) &&
                        pickup.Sequence < dropoff.Sequence)));
            }
            else if (request.PickupLocationId.HasValue)
            {
                var pickupId = request.PickupLocationId.Value;

                query = query.Where(o => o.RoutePoints.Any(rp =>
                    rp.LocationId == pickupId &&
                    pickupStopTypes.Contains(rp.StopType)));
            }
            else if (request.DropoffLocationId.HasValue)
            {
                var dropoffId = request.DropoffLocationId.Value;

                query = query.Where(o => o.RoutePoints.Any(rp =>
                    rp.LocationId == dropoffId &&
                    dropoffStopTypes.Contains(rp.StopType)));
            }

            var totalRecords = await query.CountAsync();

            var offers = await query
                .OrderBy(o => o.DepartureTime)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync();

            var data = new List<GetOfferSharedRideDTO>();
            foreach (var offer in offers)
            {
                var dto = _mapper.Map<GetOfferSharedRideDTO>(offer);

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

            return new PagedResult<GetOfferSharedRideDTO>
            {
                Items = data,
                TotalCount = totalRecords,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<GetOfferDTO> CancelOfferAsync(Guid driverId, Guid offerId)
        {
            var offer = await GetOfferWithRelationsQuery()
                                .FirstOrDefaultAsync(x => x.Id == offerId);

            if (offer == null || offer.DeleteAt != null || offer.DriverId != driverId)
            {
                throw new AppException(404, "Không tìm thấy bản ghi hoặc bạn không có quyền hủy");
            }

            if (offer.Status != OfferStatus.Active)
            {
                throw new AppException(400, "Chỉ có thể hủy Offer ở trạng thái Active");
            }

            offer.Status = OfferStatus.Closed;

            var relatedBookings = await _bookingRepo.GetQueryable()
                                            .Where(b => b.OfferId == offerId && b.DeleteAt == null)
                                            .ToListAsync();

            foreach (var booking in relatedBookings)
            {
                booking.Status = BookingStatus.Cancelled;
                _bookingRepo.Update(booking);
            }

            _repo.Update(offer);
            await _repo.SaveChangesAsync();

            return _mapper.Map<GetOfferDTO>(offer);
        }

        private IQueryable<Offer> GetOfferWithRelationsQuery()
        {
            return _repo.GetQueryable()
                .Include(o => o.Driver)
                .Include(o => o.Vehicle)
                .Include(o => o.SharedRideDetail);
        }
    }
}
