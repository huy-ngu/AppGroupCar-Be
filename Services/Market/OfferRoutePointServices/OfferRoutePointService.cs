using AppGroupCar.DTO.RequestDTO.Market.OfferRoutePoint;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Market.OfferRoutePointServices
{
    public class OfferRoutePointService : IOfferRoutePointService
    {
        private readonly IBaseRepository<OfferRoutePoint> _repo;

        public OfferRoutePointService(IBaseRepository<OfferRoutePoint> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<OfferRoutePoint>> GetAllAsync()
        {
            return await _repo.GetQueryable()
                .Where(x => x.DeleteAt == null)
                .ToListAsync();
        }

        public async Task<OfferRoutePoint> CreateAsync(CreateOfferRoutePointRequestDTO request)
        {
            ValidateCreateRequest(request);

            var newOfferRoutePoint = new OfferRoutePoint
            {
                Id = Guid.NewGuid(),
                OfferId = request.OfferId,
                Sequence = request.Sequence,
                LocationId = request.LocationId,
                PickupAllowed = request.PickupAllowed,
                DropoffAllowed = request.DropoffAllowed,
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
                offerRoutePoint.LocationId = request.LocationId.Value;
            }

            if (request.PickupAllowed.HasValue)
            {
                offerRoutePoint.PickupAllowed = request.PickupAllowed.Value;
            }

            if (request.DropoffAllowed.HasValue)
            {
                offerRoutePoint.DropoffAllowed = request.DropoffAllowed.Value;
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
