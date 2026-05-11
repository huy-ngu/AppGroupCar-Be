using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppGroupCar.Services.Mytoys.LocationServices
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
        public async Task<Location> CreateLocationAsync(CreateLocationRequestDTO request)
        {
            var newLocation = new Location
            {
                Name = request.Name,
                Address = request.Address,
                ParentLocationId = request.ParentLocationId,
                LocationType = request.LocationType,
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

        public async Task<PagedResult<LocationSearchResultDTO>> SearchLocationsAsync(LocationSearchParameters parameters)
        {
            var keyword = parameters.Keyword?.Trim();
            var pageNumber = parameters.PageNumber;
            var pageSize = parameters.PageSize;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new PagedResult<LocationSearchResultDTO>
                {
                    Items = new List<LocationSearchResultDTO>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            var query = _repo.GetQueryable()
                .AsNoTracking()
                .Where(x => x.DeleteAt == null)
                .Where(x =>
                    x.Name.Contains(keyword) ||
                    (x.Code != null && x.Code.Contains(keyword)) ||
                    (x.Address != null && x.Address.Contains(keyword)));

            if (parameters.Type.HasValue)
            {
                query = query.Where(x => x.LocationType == parameters.Type.Value);
            }

            if (parameters.ParentLocationId.HasValue)
            {
                query = query.Where(x => x.ParentLocationId == parameters.ParentLocationId.Value);
            }

            if (parameters.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == parameters.IsActive.Value);
            }

            var totalCount = await query.CountAsync();

            var locations = await query
                .OrderBy(x => x.Name == keyword ? 0 : x.Name.StartsWith(keyword) ? 1 : 2)
                .ThenBy(x => x.LocationType)
                .ThenBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var locationLookup = await LoadLocationsWithAncestorsAsync(locations);
            var items = locations
                .Select(location => BuildSearchResult(location, locationLookup))
                .ToList();

            return new PagedResult<LocationSearchResultDTO>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task DeleteLocationAsync(Guid id)
        {
            var location = await _repo.GetByIdAsync(id);
            if (location == null) return;

            _repo.Delete(location);

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

        private async Task<Dictionary<Guid, Location>> LoadLocationsWithAncestorsAsync(IEnumerable<Location> locations)
        {
            var lookup = locations.ToDictionary(x => x.Id);
            var pendingParentIds = locations
                .Where(x => x.ParentLocationId.HasValue)
                .Select(x => x.ParentLocationId!.Value)
                .Where(id => !lookup.ContainsKey(id))
                .Distinct()
                .ToList();

            while (pendingParentIds.Count > 0)
            {
                var parents = await _repo.GetQueryable()
                    .AsNoTracking()
                    .Where(x => pendingParentIds.Contains(x.Id) && x.DeleteAt == null)
                    .ToListAsync();

                if (parents.Count == 0)
                {
                    break;
                }

                foreach (var parent in parents)
                {
                    lookup.TryAdd(parent.Id, parent);
                }

                pendingParentIds = parents
                    .Where(x => x.ParentLocationId.HasValue)
                    .Select(x => x.ParentLocationId!.Value)
                    .Where(id => !lookup.ContainsKey(id))
                    .Distinct()
                    .ToList();
            }

            return lookup;
        }

        private static LocationSearchResultDTO BuildSearchResult(
            Location location,
            IReadOnlyDictionary<Guid, Location> locationLookup)
        {
            var parents = GetParentChain(location, locationLookup)
                .Select(ToBrief)
                .ToList();
            var locationDto = ToBrief(location);
            var hierarchy = parents
                .Concat(new[] { locationDto })
                .ToList();

            return new LocationSearchResultDTO
            {
                Location = locationDto,
                ParentLocations = parents,
                Hierarchy = hierarchy,
                DisplayName = BuildDisplayName(locationDto, parents),
                Path = string.Join(" > ", hierarchy.Select(x => x.Name))
            };
        }

        private static List<Location> GetParentChain(
            Location location,
            IReadOnlyDictionary<Guid, Location> locationLookup)
        {
            var parents = new List<Location>();
            var current = location;
            var visitedIds = new HashSet<Guid> { location.Id };

            while (current.ParentLocationId.HasValue &&
                   locationLookup.TryGetValue(current.ParentLocationId.Value, out var parent) &&
                   visitedIds.Add(parent.Id))
            {
                parents.Add(parent);
                current = parent;
            }

            parents.Reverse();
            return parents;
        }

        private static LocationBriefResponseDTO ToBrief(Location location)
        {
            return new LocationBriefResponseDTO
            {
                Id = location.Id,
                Name = location.Name,
                Code = location.Code,
                LocationType = location.LocationType,
                LocationTypeName = location.LocationType.ToString(),
                ParentLocationId = location.ParentLocationId,
                Address = location.Address,
                IsActive = location.IsActive,
                Latitude = location.Coordinate?.Y,
                Longitude = location.Coordinate?.X
            };
        }

        private static string BuildDisplayName(
            LocationBriefResponseDTO location,
            IReadOnlyList<LocationBriefResponseDTO> parents)
        {
            var parts = new List<string> { location.Name };
            parts.AddRange(parents.AsEnumerable().Reverse().Select(x => x.Name));
            return string.Join(", ", parts);
        }
    }
}
