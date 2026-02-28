using AppGroupCar.Entities;
using AppGroupCar.Entities.Market;
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
                // CreatedAt sẽ tự động được gán theo logic trong BaseEntity/BaseRepository
            };

            await _repo.AddAsync(newLocation);

            // NHỚ GỌI HÀM NÀY! Không gọi là database không có gì đâu nhé!
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
            if (location == null) return; // Không có thì thôi, khỏi xóa

            _repo.Delete(location);

            // Lưu thay đổi
            await _repo.SaveChangesAsync();
        }
    }
}
