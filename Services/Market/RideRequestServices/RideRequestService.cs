using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.RideRequest;
using AppGroupCar.DTO.ResponseDTO.Market.RideRequest;
using AppGroupCar.DTO.ResponseDTO.Shared;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Market.RideRequestServices
{
    public class RideRequestService : IRideRequestService
    {
        private readonly IBaseRepository<RideRequest> _repo;

        public RideRequestService(IBaseRepository<RideRequest> repo)
        {
            _repo = repo;
        }

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

        public async Task<PagedResult<RideRequestFilterResponseDTO>> FilterByTimeAndLocationAsync(RideRequestParameters parameters)
        {
            var query = _repo.GetQueryable()
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.FromLocation)
                .Include(r => r.ToLocation)
                .Where(r =>
                    (!parameters.FromLocationId.HasValue || r.FromLocationId == parameters.FromLocationId.Value) &&
                    (!parameters.ToLocationId.HasValue || r.ToLocationId == parameters.ToLocationId.Value) &&
                    (!parameters.FromTime.HasValue || r.DesiredDepartureTime >= parameters.FromTime.Value) &&
                    (!parameters.ToTime.HasValue || r.DesiredDepartureTime <= parameters.ToTime.Value) &&
                    r.Status == RideRequestStatus.Open &&
                    r.DeleteAt == null);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.DesiredDepartureTime)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(r => new RideRequestFilterResponseDTO
                {
                    Id = r.Id,
                    CreatedAt = r.CreatedAt,
                    LastUpdateAt = r.LastUpdateAt,
                    DeleteAt = r.DeleteAt,
                    Customer = new CustomerSummaryDTO
                    {
                        Id = r.Customer.Id,
                        FullName = r.Customer.FullName,
                        PhoneNumber = r.Customer.PhoneNumber,
                        AvatarUrl = r.Customer.AvatarUrl
                    },
                    FromLocation = new LocationSummaryDTO
                    {
                        Id = r.FromLocation.Id,
                        Name = r.FromLocation.Name
                    },
                    ToLocation = new LocationSummaryDTO
                    {
                        Id = r.ToLocation.Id,
                        Name = r.ToLocation.Name
                    },
                    DesiredDepartureTime = r.DesiredDepartureTime,
                    SeatsRequired = r.SeatsRequired,
                    Status = r.Status
                })
                .ToListAsync();

            return new PagedResult<RideRequestFilterResponseDTO>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
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
