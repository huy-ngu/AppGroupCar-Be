using AppGroupCar.Entities.Market;
using AppGroupCar.Repositories.BaseRepo;

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
            return await _repo.GetAllAsync();
        }
    }
}
