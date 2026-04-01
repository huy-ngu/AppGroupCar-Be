using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO.Market.Offer;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;
using System;
using AutoMapper;

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
                                            .Where(o => o.DriverId == driverId && o.Status == OfferStatus.Active)
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

            foreach (var item in request.OfferRoutePoints)
            {
                int i = 1;
                var newOfferRoutePoint = new OfferRoutePoint
                {

                    Id = Guid.NewGuid(),
                    OfferId = offer.Id,
                    Sequence = i,
                    LocationId = item.LocationId
                };
                await _routePointRepo.AddAsync(newOfferRoutePoint);
                i++;
            }

            var newCargo = new CargoOfferDetail
            {
                Id = offer.Id,
                MaxVolume = request.Cargo.MaxVolume,
                MaxWeight = request.Cargo.MaxWeight,
                AcceptFragile = request.Cargo.AcceptFragile,
            };

            await _offerRepo.AddAsync(offer);
            await _cargoRepo.AddAsync(newCargo);

            await _offerRepo.SaveChangesAsync();
            var offerResponse = _mapper.Map<GetOfferDTO>(offer);

            return offerResponse;

        }
    }
}
