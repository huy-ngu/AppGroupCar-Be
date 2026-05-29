using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO.Market.RideRequest;
using AppGroupCar.DTO.Filters;
using AppGroupCar.Entities.Market;
using AppGroupCar.Repositories.BaseRepo;

namespace AppGroupCar.Services.Market.RideRequestServices
{
    public interface IRideRequestService
    {
        public Task<RideRequest> CreateRideRequestAsync(Guid userId, CreateRideRequestDTO rideRequest);
        public Task<IEnumerable<RideRequest>> GetAllRideRequestAsync();
        public Task<RideRequest> GetByIdAsync(Guid id);
        public Task<PagedResult<RideRequest>> FindByNameAsync(string name, int pageNumber, int PageSize);
        public Task<PagedResult<RideRequest>> FilterByTimeAndLocationAsync(RideRequestParameters parameters);
        public Task DeleteByIdAsync(Guid id);
        public Task UpdateAsync(UpdateRideRequestDTO rideRequest);
    }
}
