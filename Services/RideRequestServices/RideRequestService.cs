using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;

namespace AppGroupCar.Services.RideRequestServices
{
    public class RideRequestService : IRideRequestService
    {
        private readonly IBaseRepository<RideRequest> _repo;

        public RideRequestService(IBaseRepository<RideRequest> repo) { _repo = repo; }

        public async Task<RideRequest> CreateRideRequestAsync(Guid userId, CreateRideRequestDTO rideRequest)
        {
            var newRideReques = new RideRequest
            {
                CustomerId = userId,
                FromLocationId = rideRequest.FromLocationId,
                ToLocationId = rideRequest.ToLocationId,
                DesiredDepartureTime = rideRequest.DesiredDepartureTime,
                SeatsRequired = rideRequest.SeatsRequired,
                Status = RideRequestStatus.Open,
            };

            await _repo.AddAsync(newRideReques);

            int rowChange = await _repo.SaveChangesAsync();

            if (rowChange <= 0)
            {
                throw new AppException(500, "Có lỗi ở Database");
            }

            return newRideReques;
        }

        public async Task<PagedResult<RideRequest>> FilterByTimeAndLocationAsync(
            string? fromLocation,
            string? toLocation,
            DateTime? fromTime,
            DateTime? toTime)
        {
            var data = await _repo.FindAndFilter(
                predicate: r =>
                    //  Lọc điểm đi (nếu null/rỗng thì bỏ qua điều kiện này)
                    (string.IsNullOrEmpty(fromLocation) || r.FromLocation.Name.Contains(fromLocation)) &&

                    // Lọc điểm đến (đã sửa lại thành ToLocation)
                    (string.IsNullOrEmpty(toLocation) || r.ToLocation.Name.Contains(toLocation)) &&

                    // Lọc thời gian bắt đầu (chỉ lấy cuốc xe đi SAU FromTime)
                    (!fromTime.HasValue || r.DesiredDepartureTime >= fromTime.Value) &&

                    // Lọc thời gian kết thúc (chỉ lấy cuốc xe đi TRƯỚC ToTime)
                    (!toTime.HasValue || r.DesiredDepartureTime <= toTime.Value) &&

                    // Mặc định luôn chỉ lấy cuốc xe đang Open
                    r.Status == RideRequestStatus.Open,

                orderBy: q => q.OrderBy(r => r.DesiredDepartureTime),
                pageNumber: 1,
                pageSize: 10
            );

            return data;
        }

        public async Task<PagedResult<RideRequest>> FindByNameAsync(string name, int pageNumber, int pageSize)
        {
            var data = await _repo.FindAndFilter(
                predicate: r => r.Customer.FullName.Contains(name),
                orderBy: q => q.OrderByDescending(r => r.DesiredDepartureTime),
                pageNumber,
                pageSize
            );
            return data;
        }

        public async Task<IEnumerable<RideRequest>> GetAllRideRequestAsync()
        {
            var RideRequestList = await _repo.GetAllAsync();
            return RideRequestList;
        }

    }
}
