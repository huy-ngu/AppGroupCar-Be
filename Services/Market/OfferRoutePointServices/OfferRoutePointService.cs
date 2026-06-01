using AppGroupCar.DTO.RequestDTO.Market.OfferRoutePoint;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Services.RouteRules;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Market.OfferRoutePointServices
{
    public class OfferRoutePointService : IOfferRoutePointService
    {
        private readonly IBaseRepository<OfferRoutePoint> _repo;
        private readonly IRouteValidationService _routeValidationService;
        private readonly IMapper _mapper;

        public OfferRoutePointService(
            IBaseRepository<OfferRoutePoint> repo,
            IRouteValidationService routeValidationService,
            IMapper mapper)
        {
            _repo = repo;
            _routeValidationService = routeValidationService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OfferRoutePoint>> GetAllAsync()
        {
            return await _repo.GetQueryable()
                .Where(x => x.DeleteAt == null)
                .OrderBy(x => x.OfferId)
                .ThenBy(x => x.Sequence)
                .ToListAsync();
        }

        public async Task<IEnumerable<GetOfferRoutePointDTO>> GetByOfferIdOrderedAsync(Guid offerId)
        {
            if (offerId == Guid.Empty)
            {
                throw new AppException(400, "Invalid offer id");
            }

            var routePoints = await _repo.GetQueryable()
                .Where(x => x.OfferId == offerId && x.DeleteAt == null)
                .Include(x => x.Location)
                .OrderBy(x => x.Sequence)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GetOfferRoutePointDTO>>(routePoints);
        }

        public async Task<OfferRoutePoint> CreateAsync(CreateOfferRoutePointRequestDTO request)
        {
            ValidateCreateRequest(request);
            await _routeValidationService.EnsureLocationAllowedForRouteAsync(request.LocationId);

            var newOfferRoutePoint = new OfferRoutePoint
            {
                Id = Guid.NewGuid(),
                OfferId = request.OfferId,
                Sequence = request.Sequence,
                LocationId = request.LocationId,
                StopType = request.StopType,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(newOfferRoutePoint);
            await _repo.SaveChangesAsync();

            return newOfferRoutePoint;
        }

        public async Task UpdateAsync(Guid id, UpdateOfferRoutePointRequestDTO request)
        {
            if (id == Guid.Empty)
            {
                throw new AppException(400, "Invalid id");
            }

            if (request.OfferId.HasValue && request.OfferId.Value == Guid.Empty)
            {
                throw new AppException(400, "Invalid offer id");
            }

            if (request.LocationId.HasValue && request.LocationId.Value == Guid.Empty)
            {
                throw new AppException(400, "Invalid location id");
            }

            var offerRoutePoint = await _repo.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == id && x.DeleteAt == null);

            if (offerRoutePoint == null)
            {
                throw new AppException(404, "Offer route point not found");
            }

            if (request.OfferId.HasValue)
            {
                offerRoutePoint.OfferId = request.OfferId.Value;
            }

            if (request.LocationId.HasValue)
            {
                await _routeValidationService.EnsureLocationAllowedForRouteAsync(request.LocationId.Value);
                offerRoutePoint.LocationId = request.LocationId.Value;
            }

            if (request.StopType.HasValue)
            {
                offerRoutePoint.StopType = request.StopType.Value;
            }

            _repo.Update(offerRoutePoint);
            await _repo.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new AppException(400, "Invalid id");
            }

            var offerRoutePoint = await _repo.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == id && x.DeleteAt == null);

            if (offerRoutePoint == null)
            {
                throw new AppException(404, "Offer route point not found");
            }

            _repo.Delete(offerRoutePoint);
            await _repo.SaveChangesAsync();

            return true;
        }

        private static void ValidateCreateRequest(CreateOfferRoutePointRequestDTO request)
        {
            if (request.OfferId == Guid.Empty)
            {
                throw new AppException(400, "Invalid offer id");
            }

            if (request.LocationId == Guid.Empty)
            {
                throw new AppException(400, "Invalid location id");
            }

            if (request.Sequence <= 0)
            {
                throw new AppException(400, "Sequence must be greater than 0");
            }
        }
    }
}
