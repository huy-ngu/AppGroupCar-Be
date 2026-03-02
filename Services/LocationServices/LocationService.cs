using System.Linq.Expressions;
using AppGroupCar.Entities;
using Microsoft.EntityFrameworkCore;
using AppGroupCar.Repositories.BaseRepo;

namespace AppGroupCar.Services.LocationServices
{
    public class LocationService
    {
        // Khai báo IBaseRepository với kiểu T là Vehicle
        private readonly IBaseRepository<Location> _repo;

        // Tiêm (Inject) qua Constructor
        public LocationService(IBaseRepository<Location> Repo)
        {
            _repo = Repo ?? throw new ArgumentNullException(nameof(Repo));
        }

        /// <summary>
        /// Thêm một xe mới
        /// </summary>
        public async Task<Location> CreateLocationAsync(string name, LocationType type)
        {
            var newLocation = new Location
            {
                Name = name,
                LocationType = type
            };

            await _repo.AddAsync(newLocation);

            await _repo.SaveChangesAsync();

            return newLocation;
        }

        /// <summary>
        /// Lấy xe theo ID
        /// </summary>
        public async Task<Location?> GetLocationByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }
        /// <summary>
        /// Lấy All
        /// </summary>
        public async Task<IEnumerable<Location?>> GetAllAsync()
        {
            return await _repo.GetAllAsync();

        }
        /// <summary>
        /// Xóa xe (Soft Delete)
        /// </summary>
        public async Task DeleteLocationAsync(Guid id)
        {
            var location = await _repo.GetByIdAsync(id);
            if (location == null) return;

            _repo.Delete(location);

            // Lưu thay đổi
            await _repo.SaveChangesAsync();
        }

        /// <summary>
        /// Tìm kiếm xe với nhiều điều kiện (keyword, type, parentLocationId, isActive) và phân trang
        /// </summary>
        //Tuy nhiên có thể phát triển thêm phần tìm kiếm nâng cao như: tìm theo tọa độ (gần đây nhất), tìm theo GooglePlaceId, v.v... tùy nhu cầu
        public async Task<IEnumerable<Location>> FindLocationsAsync(
    string? keyword,
    LocationType? type,
    Guid? parentLocationId,
    bool? isActive,
    int pageNumber,
    int pageSize)
        {
            Expression<Func<Location, bool>> predicate = x =>
                (string.IsNullOrWhiteSpace(keyword) ||
                    EF.Functions.Like(x.Name, $"%{keyword}%")) &&
                (!type.HasValue || x.LocationType == type.Value) &&
                (!parentLocationId.HasValue || x.ParentLocationId == parentLocationId) &&
                (!isActive.HasValue || x.IsActive == isActive.Value);

            return await _repo.FindAsync(predicate, pageNumber, pageSize);
        }
    }
}
