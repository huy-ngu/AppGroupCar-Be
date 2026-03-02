using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using System.Data;

namespace AppGroupCar.Services.RideRequestServices
{
    public class RideRequestService : IRideRequestService
    {
        private readonly IBaseRepository<RideRequest> _repo;

        public RideRequestService(IBaseRepository<RideRequest> repo) { _repo = repo; }

        public async Task<RideRequest> createRideRequestAsync(Guid userId, CreateRideRequestDTO rideRequest)
        {
            var newRideReques = new RideRequest
            {
                CustomerId = userId,
                FromLocationId = rideRequest.FromLocationId,
                ToLocationId = rideRequest.ToLocationId,
                DesiredDepartureTime = rideRequest.DesiredDepartureTime,
                SeatsRequired = rideRequest.SeatsRequired,
            };

            await _repo.AddAsync(newRideReques);

            int rowChange = await _repo.SaveChangesAsync();

            if (rowChange <= 0)
            {
                throw new AppException(500, "Có lỗi ở Database");
            }

            return newRideReques;
        }
    }
}
