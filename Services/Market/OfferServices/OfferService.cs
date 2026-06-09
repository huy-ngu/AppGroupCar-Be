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
        private readonly IBaseRepository<ShipmentOffer> _shipmentOfferRepo;
        private readonly IBaseRepository<Shipment> _shipmentRepo;
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
            IBaseRepository<ShipmentOffer> shipmentOfferRepo,
            IBaseRepository<Shipment> shipmentRepo,
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
            _shipmentOfferRepo = shipmentOfferRepo;
            _shipmentRepo = shipmentRepo;
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
                .OrderByDescending(x => x.DepartureTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetOfferDTO>>(offers);
        }

        public async Task<IEnumerable<GetOfferDTO>> GetMyOffersAsync(Guid driverId)
        {
            var offers = await _repo.GetQueryable()
                .Where(o => o.DriverId == driverId && o.DeleteAt == null)
                .Include(o => o.SharedRideDetail)
                .Include(o => o.RoutePoints)
                    .ThenInclude(rp => rp.Location)
                .Include(o => o.Driver)
                .Include(o => o.Vehicle)
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetOfferDTO>>(offers);
        }

        public async Task<IEnumerable<GetOfferDTO>> GetMyActiveSharedRideOffersAsync(Guid driverId)
        {
            var offers = await GetOfferWithRelationsQuery()
                .Where(o =>
                    o.DriverId == driverId &&
                    o.DeleteAt == null &&
                    o.Status == OfferStatus.Active &&
                    o.ServiceType == ServiceType.Shared)
                .OrderByDescending(o => o.DepartureTime)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetOfferDTO>>(offers);
        }

        public async Task<IEnumerable<GetOfferDTO>> GetMyActiveShipmentOffersAsync(Guid driverId)
        {
            var offers = await GetOfferWithRelationsQuery()
                .Where(o =>
                    o.DriverId == driverId &&
                    o.DeleteAt == null &&
                    o.Status == OfferStatus.Active &&
                    o.ServiceType == ServiceType.Truck)
                .OrderByDescending(o => o.DepartureTime)
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

        public async Task<GetOfferSharedRideDetailDTO> GetSharedRideDetailAsync(Guid id)
        {
            var offer = await _repo.GetQueryable()
                .AsNoTracking()
                .Include(o => o.SharedRideDetail)
                .Include(o => o.Driver)
                    .ThenInclude(d => d.User)
                .Include(o => o.Vehicle)
                .Include(o => o.RoutePoints)
                    .ThenInclude(rp => rp.Location)
                .FirstOrDefaultAsync(o =>
                    o.Id == id &&
                    o.ServiceType == ServiceType.Shared &&
                    o.DeleteAt == null);

            if (offer == null)
            {
                throw new AppException(404, "Không tìm thấy chuyến xe ghép");
            }

            if (offer.SharedRideDetail == null)
            {
                throw new AppException(404, "Không tìm thấy thông tin chi tiết chuyến xe ghép");
            }

            return new GetOfferSharedRideDetailDTO
            {
                Id = offer.Id,
                DriverId = offer.DriverId,
                VehicleId = offer.VehicleId,
                ServiceType = offer.ServiceType,
                DepartureTime = offer.DepartureTime,
                EstimatedDurationMinutes = offer.EstimatedDurationMinutes,
                BasePrice = offer.BasePrice,
                Status = offer.Status,
                SharedRideDetail = new SharedRideDetailInfoDTO
                {
                    TotalSeats = offer.SharedRideDetail.TotalSeats,
                    AvailableSeats = offer.SharedRideDetail.AvailableSeats
                },
                Driver = new SharedRideDriverInfoDTO
                {
                    DriverId = offer.DriverId,
                    Name = offer.Driver.Name,
                    LicenseNumber = offer.Driver.LicenseNumber,
                    LicenseClass = offer.Driver.LicenseClass,
                    RatingAverage = offer.Driver.DriverRatingAverage,
                    RatingCount = offer.Driver.DriverRatingCount,
                    AvatarUrl = offer.Driver.User?.AvatarUrl,
                    PhoneNumber = offer.Driver.User?.PhoneNumber
                },
                Vehicle = new SharedRideVehicleInfoDTO
                {
                    VehicleId = offer.VehicleId,
                    PlateNumber = offer.Vehicle.PlateNumber,
                    Brand = offer.Vehicle.Brand,
                    SeatCapacity = offer.Vehicle.SeatCapacity,
                    VehicleType = offer.Vehicle.VehicleType,
                    Status = offer.Vehicle.Status,
                    UrlImage = offer.Vehicle.UrlImage
                },
                RoutePoints = offer.RoutePoints
                    .OrderBy(rp => rp.Sequence)
                    .Select(rp => new SharedRideRoutePointInfoDTO
                    {
                        RoutePointId = rp.Id,
                        LocationId = rp.LocationId,
                        LocationName = rp.Location?.Name ?? "",
                        Address = rp.Location?.Address,
                        LocationType = rp.Location?.LocationType ?? LocationType.Other,
                        StopType = rp.StopType,
                        Sequence = rp.Sequence
                    })
                    .ToList()
            };
        }

        public async Task<GetOfferShipmentDetailDTO> GetShipmentDetailAsync(Guid id)
        {
            var offer = await _repo.GetQueryable()
                .AsNoTracking()
                .Include(o => o.CargoDetail)
                .Include(o => o.Driver)
                    .ThenInclude(d => d.User)
                .Include(o => o.Vehicle)
                .Include(o => o.RoutePoints)
                    .ThenInclude(rp => rp.Location)
                .FirstOrDefaultAsync(o =>
                    o.Id == id &&
                    o.ServiceType == ServiceType.Truck &&
                    o.DeleteAt == null);

            if (offer == null)
            {
                throw new AppException(404, "Không tìm thấy chuyến hàng");
            }

            if (offer.CargoDetail == null)
            {
                throw new AppException(404, "Không tìm thấy thông tin chi tiết chuyến hàng");
            }

            return new GetOfferShipmentDetailDTO
            {
                Id = offer.Id,
                DriverId = offer.DriverId,
                VehicleId = offer.VehicleId,
                ServiceType = offer.ServiceType,
                DepartureTime = offer.DepartureTime,
                EstimatedDurationMinutes = offer.EstimatedDurationMinutes,
                BasePrice = offer.BasePrice,
                Status = offer.Status,
                CargoDetail = new CargoDetailDTO
                {
                    MaxWeight = offer.CargoDetail.MaxWeight,
                    MaxVolume = offer.CargoDetail.MaxVolume,
                    AcceptFragile = offer.CargoDetail.AcceptFragile
                },
                Driver = new SharedRideDriverInfoDTO
                {
                    DriverId = offer.DriverId,
                    Name = offer.Driver.Name,
                    LicenseNumber = offer.Driver.LicenseNumber,
                    LicenseClass = offer.Driver.LicenseClass,
                    RatingAverage = offer.Driver.DriverRatingAverage,
                    RatingCount = offer.Driver.DriverRatingCount,
                    AvatarUrl = offer.Driver.User?.AvatarUrl,
                    PhoneNumber = offer.Driver.User?.PhoneNumber
                },
                Vehicle = new SharedRideVehicleInfoDTO
                {
                    VehicleId = offer.VehicleId,
                    PlateNumber = offer.Vehicle.PlateNumber,
                    Brand = offer.Vehicle.Brand,
                    SeatCapacity = offer.Vehicle.SeatCapacity,
                    VehicleType = offer.Vehicle.VehicleType,
                    Status = offer.Vehicle.Status,
                    UrlImage = offer.Vehicle.UrlImage
                },
                RoutePoints = offer.RoutePoints
                    .OrderBy(rp => rp.Sequence)
                    .Select(rp => new SharedRideRoutePointInfoDTO
                    {
                        RoutePointId = rp.Id,
                        LocationId = rp.LocationId,
                        LocationName = rp.Location?.Name ?? "",
                        Address = rp.Location?.Address,
                        LocationType = rp.Location?.LocationType ?? LocationType.Other,
                        StopType = rp.StopType,
                        Sequence = rp.Sequence
                    })
                    .ToList()
            };
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
            return await CancelOfferSharedRideAsync(driverId, offerId);
        }

        public async Task<GetOfferDTO> CancelOfferSharedRideAsync(Guid driverId, Guid offerId)
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

        public async Task<GetOfferDTO> CancelOfferShipmentAsync(Guid driverId, Guid offerId)
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

            var relatedShipments = await _shipmentRepo.GetQueryable()
                .Include(s => s.ShipmentOffer)
                .Where(s => s.DeleteAt == null && s.ShipmentOffer != null && s.ShipmentOffer.OfferId == offerId)
                .ToListAsync();

            foreach (var shipment in relatedShipments)
            {
                shipment.ShipmentStatus = ShipmentStatus.Cancelled;
                _shipmentRepo.Update(shipment);
            }

            var relatedShipmentOffers = await _shipmentOfferRepo.GetQueryable()
                .Where(so => so.DeleteAt == null && so.OfferId == offerId)
                .ToListAsync();

            foreach (var shipmentOffer in relatedShipmentOffers)
            {
                shipmentOffer.Status = ShipmentOfferStatus.Cancelled;
                _shipmentOfferRepo.Update(shipmentOffer);
            }

            _repo.Update(offer);
            await _repo.SaveChangesAsync();

            return _mapper.Map<GetOfferDTO>(offer);
        }

        public async Task<GetOfferDTO> CompleteOfferSharedRideAsync(Guid driverId, Guid offerId)
        {
            var offer = await GetOfferWithRelationsQuery()
                                .FirstOrDefaultAsync(x => x.Id == offerId);

            if (offer == null || offer.DeleteAt != null || offer.DriverId != driverId)
            {
                throw new AppException(404, "Không tìm thấy bản ghi hoặc bạn không có quyền hoàn thành");
            }

            if (offer.Status != OfferStatus.Active)
            {
                throw new AppException(400, "Chỉ có thể hoàn thành Offer ở trạng thái Active");
            }

            offer.Status = OfferStatus.Complete;

            var relatedBookings = await _bookingRepo.GetQueryable()
                                            .Where(b => b.OfferId == offerId && b.DeleteAt == null && b.Status == BookingStatus.Accepted)
                                            .ToListAsync();

            foreach (var booking in relatedBookings)
            {
                booking.Status = BookingStatus.Compelete;
                _bookingRepo.Update(booking);
            }

            _repo.Update(offer);
            await _repo.SaveChangesAsync();

            return _mapper.Map<GetOfferDTO>(offer);
        }

        public async Task<GetOfferDTO> CompleteOfferShipmentAsync(Guid driverId, Guid offerId)
        {
            var offer = await GetOfferWithRelationsQuery()
                                .FirstOrDefaultAsync(x => x.Id == offerId);

            if (offer == null || offer.DeleteAt != null || offer.DriverId != driverId)
            {
                throw new AppException(404, "Không tìm thấy bản ghi hoặc bạn không có quyền hoàn thành");
            }

            if (offer.Status != OfferStatus.Active)
            {
                throw new AppException(400, "Chỉ có thể hoàn thành Offer ở trạng thái Active");
            }

            offer.Status = OfferStatus.Complete;

            var relatedShipments = await _shipmentRepo.GetQueryable()
                .Include(s => s.ShipmentOffer)
                .Where(s => s.DeleteAt == null && s.ShipmentOffer != null && s.ShipmentOffer.OfferId == offerId )
                .ToListAsync();

            foreach (var shipment in relatedShipments)
            {
                shipment.ShipmentStatus = ShipmentStatus.Delivered;
                shipment.DeliveredDate = DateTime.Now;
                _shipmentRepo.Update(shipment);
            }

            _repo.Update(offer);
            await _repo.SaveChangesAsync();

            return _mapper.Map<GetOfferDTO>(offer);
        }

        public async Task<OfferDetailResponseDto> GetOfferDetailAsync(Guid id)
        {
            var offer = await _repo.GetQueryable()
                .Include(o => o.Driver)
                    .ThenInclude(d => d.User)
                .Include(o => o.Vehicle)
                .Include(o => o.SharedRideDetail)
                .Include(o => o.RoutePoints)
                    .ThenInclude(rp => rp.Location)
                .FirstOrDefaultAsync(o => o.Id == id && o.DeleteAt == null);

            if (offer == null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }

            var routePoints = offer.RoutePoints
                .Where(rp => rp.DeleteAt == null)
                .OrderBy(rp => rp.Sequence)
                .Select(rp => new OfferRoutePointResponseDto
                {
                    Id = rp.Id,
                    LocationId = rp.LocationId,
                    LocationName = rp.Location?.Name ?? string.Empty,
                    Sequence = rp.Sequence,
                    StopType = rp.StopType
                })
                .ToList();

            var startLocation = routePoints.FirstOrDefault(rp => rp.StopType == RouteStopType.Start)?.LocationName;
            var endLocation = routePoints.FirstOrDefault(rp => rp.StopType == RouteStopType.End)?.LocationName;
            var routeName = (startLocation != null && endLocation != null) ? $"{startLocation} - {endLocation}" : $"Route for Offer {offer.Id}";

            var route = new OfferRouteResponseDto
            {
                Id = offer.Id,
                Name = routeName,
                EstimatedDurationMinutes = offer.EstimatedDurationMinutes,
                RoutePoints = routePoints
            };

            return new OfferDetailResponseDto
            {
                Id = offer.Id,
                DriverId = offer.DriverId,
                DepartureTime = offer.DepartureTime,
                BasePrice = offer.BasePrice,
                Status = offer.Status,
                ServiceType = offer.ServiceType,
                TotalSeats = offer.SharedRideDetail?.TotalSeats ?? 0,
                AvailableSeats = offer.SharedRideDetail?.AvailableSeats ?? 0,
                Route = route,
                Vehicle = new OfferVehicleDto
                {
                    VehicleId = offer.VehicleId,
                    PlateNumber = offer.Vehicle.PlateNumber,
                    Brand = offer.Vehicle.Brand,
                    SeatCapacity = offer.Vehicle.SeatCapacity,
                    VehicleType = (int)offer.Vehicle.VehicleType,
                    UrlImage = offer.Vehicle.UrlImage
                },
                Driver = new OfferDriverDto
                {
                    DriverId = offer.DriverId,
                    FullName = offer.Driver.Name,
                    AvatarUrl = offer.Driver.User?.AvatarUrl,
                    LicenseNumber = offer.Driver.LicenseNumber,
                    LicenseClass = offer.Driver.LicenseClass,
                    DriverRatingAverage = offer.Driver.DriverRatingAverage,
                    DriverRatingCount = offer.Driver.DriverRatingCount
                }
            };
        }

        private IQueryable<Offer> GetOfferWithRelationsQuery()
        {
            return _repo.GetQueryable()
                .Include(o => o.Driver)
                .Include(o => o.Vehicle)
                .Include(o => o.RoutePoints)
                    .ThenInclude(rp => rp.Location)
                .Include(o => o.SharedRideDetail);
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
