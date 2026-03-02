using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Repositories.BaseRepo;

namespace AppGroupCar.Services.RideRequestServices
{
    public interface IRideRequestService
    {
        public Task<RideRequest> createRideRequestAsync(Guid userId, CreateRideRequestDTO rideRequest);
    }
}
