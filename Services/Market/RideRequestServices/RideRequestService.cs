using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO.Market.RideRequest;
using AppGroupCar.DTO.Filters;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;

namespace AppGroupCar.Services.Market.RideRequestServices
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
            await _repo.SaveChangesAsync();
            return newRideReques;
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            var rideRequest = await _repo.GetByIdAsync(id);
            if (rideRequest == null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            if (rideRequest.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            _repo.Delete(rideRequest);
            await _repo.SaveChangesAsync();
        }

        public async Task<PagedResult<RideRequest>> FilterByTimeAndLocationAsync(RideRequestParameters parameters)
        {
            var data = await _repo.FindAndFilter(
                predicate: r =>
                    //  Lọc điểm đi (nếu null/rỗng thì bỏ qua điều kiện này)
                    (string.IsNullOrEmpty(parameters.FromLocation) || r.FromLocation.Name.Contains(parameters.FromLocation)) &&

                    // Lọc điểm đến (đã sửa lại thành ToLocation)
                    (string.IsNullOrEmpty(parameters.ToLocation) || r.ToLocation.Name.Contains(parameters.ToLocation)) &&

                    // Lọc thời gian bắt đầu (chỉ lấy cuốc xe đi SAU FromTime)
                    (!parameters.FromTime.HasValue || r.DesiredDepartureTime >= parameters.FromTime.Value) &&

                    // Lọc thời gian kết thúc (chỉ lấy cuốc xe đi TRƯỚC ToTime)
                    (!parameters.ToTime.HasValue || r.DesiredDepartureTime <= parameters.ToTime.Value) &&

                    // Mặc định luôn chỉ lấy cuốc xe đang Open
                    r.Status == RideRequestStatus.Open && r.DeleteAt == null,

                orderBy: q => q.OrderByDescending(r => r.DesiredDepartureTime),
                pageNumber: parameters.PageNumber,
                pageSize: parameters.PageSize
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

        public async Task<RideRequest> GetByIdAsync(Guid id)
        {
            var rideRequest = await _repo.GetByIdAsync(id);
            if (rideRequest == null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            if (rideRequest.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            return rideRequest;
        }

        public async Task UpdateAsync(UpdateRideRequestDTO request)
        {
            var existingRide = await _repo.GetByIdAsync(request.Id);

            if (existingRide == null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            if (existingRide.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            if (request.FromLocationId != null)
            {
                existingRide.FromLocationId = (Guid)request.FromLocationId;
            }
            if (request.ToLocationId != null)
            {
                existingRide.ToLocationId = (Guid)request.ToLocationId;
            }
            if (request.DesiredDepartureTime != null)
            {
                existingRide.DesiredDepartureTime = (DateTime)request.DesiredDepartureTime;
            }
            if (request.SeatsRequired != null)
            {
                existingRide.SeatsRequired = (int)request.SeatsRequired;
            }
            if (request.Status != null)
            {
                existingRide.Status = request.Status;
            }
            _repo.Update(existingRide);

            await _repo.SaveChangesAsync();
        }
    }
}
