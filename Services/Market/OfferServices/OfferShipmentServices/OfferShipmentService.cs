using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO.Market.Offer;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;
using System;
using AutoMapper;
using AppGroupCar.DTO;

namespace AppGroupCar.Services.Market.OfferServices.OfferShipmentServices
{
    public class OfferShipmentService : IOfferShipmentSerivce
    {
        private readonly IBaseRepository<Offer> _offerRepo;
        private readonly IBaseRepository<CargoOfferDetail> _cargoRepo;
        private readonly IBaseRepository<OfferRoutePoint> _routePointRepo;
        private readonly IBaseRepository<Vehicle> _vehicleRepo;
        private readonly IMapper _mapper;


        public OfferShipmentService(IBaseRepository<Offer> offerRepo, IBaseRepository<CargoOfferDetail> cargoRepo, IBaseRepository<OfferRoutePoint> routePointRepo, IBaseRepository<Vehicle> vehicleRepo, IMapper mapper)
        {
            _cargoRepo = cargoRepo;
            _routePointRepo = routePointRepo;
            _offerRepo = offerRepo;
            _vehicleRepo = vehicleRepo;
            _mapper = mapper;
        }

        public async Task<GetOfferDTO> CreateWithCargoAsync(Guid driverId, CreateOfferShipmentRequestDTO request)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId);

            if (vehicle == null || vehicle.DriverId != driverId)
            {
                throw new AppException(400, "Vehicle không tồn tại hoặc không thuộc về bạn");
            }
            if (request.DepartureTime < DateTime.UtcNow)
            {
                throw new AppException(400, "Thời gian không hợp lệ");
            }
            if (request.OfferRoutePoints.Count() < 2)
            {
                throw new AppException(400, "Offer phải có ít nhất 2 Route Point");
            }
            var recentOffer = await _offerRepo.GetQueryable()
                                            .AsNoTracking()
                                            .Where(o => o.DriverId == driverId && o.Status == OfferStatus.Active && o.DeleteAt == null)
                                            .OrderByDescending(o => o.DepartureTime)
                                            .FirstOrDefaultAsync();
            if (recentOffer != null)
            {
                throw new AppException(400, "Bạn đang có Offer hoạt động. Vui lòng hoàn thành Offer cũ để có thể tạo mới");
            }
            var offer = _mapper.Map<Offer>(request);
            offer.Id = Guid.NewGuid();
            offer.DriverId = driverId;
            offer.Status = OfferStatus.Active;
            offer.ServiceType = ServiceType.Truck;

            var newRoutePoints = request.OfferRoutePoints
                 .Select((item, index) => new OfferRoutePoint
                 {
                     Id = Guid.NewGuid(),
                     OfferId = offer.Id,
                     Sequence = index + 1,
                     LocationId = item.LocationId
                 })
                 .ToList();

            var newCargo = new CargoOfferDetail
            {
                Id = offer.Id,
                MaxVolume = request.Cargo.MaxVolume,
                MaxWeight = request.Cargo.MaxWeight,
                AcceptFragile = request.Cargo.AcceptFragile,
            };
            await _routePointRepo.AddRangeAsync(newRoutePoints);

            await _offerRepo.AddAsync(offer);
            await _cargoRepo.AddAsync(newCargo);

            await _offerRepo.SaveChangesAsync();
            var offerResponse = _mapper.Map<GetOfferDTO>(offer);

            return offerResponse;

        }

        public async Task<PagedResult<GetOfferDTO>> GetOfferShipmentAsync(GetOfferForUserRequestDTO request)
        {
            // BƯỚC 1: SỬA INCLUDE THÀNH CARGODETAIL! Nhìn cho kỹ vào!
            var query = _offerRepo.GetQueryable()
                .Include(o => o.CargoDetail)
                .Include(o => o.RoutePoints)
                .Include(o => o.Driver)
                .Include(o => o.Vehicle)
                .Where(o => o.Status == OfferStatus.Active && o.ServiceType == ServiceType.Truck);

            // BƯỚC 2: Logic lọc thời gian giữ nguyên
            if (request.startTime.HasValue)
            {
                query = query.Where(o => o.DepartureTime >= request.startTime.Value);
            }

            // BƯỚC 3: Logic lọc điểm đến/điểm đi (như đã hướng dẫn ở lần trước)
            if (request.PickupLocationId.HasValue && request.DropoffLocationId.HasValue)
            {
                query = query.Where(o => o.RoutePoints.Any(pickup =>
                    pickup.LocationId == request.PickupLocationId.Value &&
                    o.RoutePoints.Any(dropoff =>
                        dropoff.LocationId == request.DropoffLocationId.Value &&
                        pickup.Sequence < dropoff.Sequence)));
            }
            else if (request.PickupLocationId.HasValue)
            {
                query = query.Where(o => o.RoutePoints.Any(rp => rp.LocationId == request.PickupLocationId.Value));
            }
            else if (request.DropoffLocationId.HasValue)
            {
                query = query.Where(o => o.RoutePoints.Any(rp => rp.LocationId == request.DropoffLocationId.Value));
            }

            var totalRecords = await query.CountAsync();

            // BƯỚC 4: Mapping (NHỚ SỬA LẠI AUTOMAPPER NẾU CẦN THIẾT)
            var data = await query
                .OrderBy(o => o.DepartureTime)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .Select(o => _mapper.Map<GetOfferDTO>(o))
                .ToListAsync();

            return new PagedResult<GetOfferDTO>
            {
                Items = data,
                TotalCount = totalRecords,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
