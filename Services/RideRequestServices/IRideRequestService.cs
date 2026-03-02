using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Repositories.BaseRepo;

namespace AppGroupCar.Services.RideRequestServices
{
    public interface IRideRequestService
    {
        public Task<RideRequest> CreateRideRequestAsync(Guid userId, CreateRideRequestDTO rideRequest);
        public Task<IEnumerable<RideRequest>> GetAllRideRequestAsync();
        public Task<PagedResult<RideRequest>> FindByNameAsync(string name, int pageNumber, int PageSize);
        public Task<PagedResult<RideRequest>> FilterByTimeAndLocationAsync(string Fromlocation, string ToLocation, DateTime? FromTime, DateTime? ToTime);

    }
}
