using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppGroupCar.Services.Mytoys.LocationServices
{
    public class LocationService
    {
        private const string AccentInsensitiveCollation = "Latin1_General_100_CI_AI";
        private readonly IBaseRepository<Location> _repo;

        public LocationService(IBaseRepository<Location> repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

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

        public async Task<Location?> GetLocationByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Location?>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<PagedResult<LocationSearchResultDTO>> SearchLocationsAsync(LocationSearchParameters parameters)
        {
            var keyword = parameters.Query?.Trim();
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

            var containsPattern = BuildLikePattern(keyword, searchAnywhere: true);
            var startsWithPattern = BuildLikePattern(keyword, searchAnywhere: false);

            var query = _repo.GetQueryable()
                .AsNoTracking()
                .Where(x => x.DeleteAt == null)
                .Where(x => x.IsActive == parameters.IsActive)
                .Where(x =>
                    EF.Functions.Like(EF.Functions.Collate(x.Name, AccentInsensitiveCollation), containsPattern) ||
                    (x.Code != null &&
                     EF.Functions.Like(EF.Functions.Collate(x.Code, AccentInsensitiveCollation), containsPattern)));

            var totalCount = await query.CountAsync();

            var locations = await query
                .OrderBy(x =>
                    EF.Functions.Collate(x.Name, AccentInsensitiveCollation) == keyword
                        ? 0
                        : x.Code != null && EF.Functions.Collate(x.Code, AccentInsensitiveCollation) == keyword
                            ? 1
                            : EF.Functions.Like(EF.Functions.Collate(x.Name, AccentInsensitiveCollation), startsWithPattern)
                                ? 2
                                : 3)
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
            if (location == null)
            {
                return;
            }

            _repo.Delete(location);
            await _repo.SaveChangesAsync();
        }

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
            var parents = GetParentChain(location, locationLookup);

            var displayParts = new List<string> { location.Name };
            displayParts.AddRange(parents
                .AsEnumerable()
                .Reverse()
                .Select(x => x.Name));

            return new LocationSearchResultDTO
            {
                Id = location.Id,
                Code = location.Code,
                Name = location.Name,
                LocationType = location.LocationType,
                LocationTypeName = location.LocationType.ToString(),
                LocationTypeLabel = GetLocationTypeLabel(location.LocationType),
                DisplayName = string.Join(", ", displayParts)
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

            return parents;
        }

        private static string FormatLocationSegment(Location location)
        {
            return $"{GetLocationTypeLabel(location.LocationType)} {location.Name}";
        }

        private static string GetLocationTypeLabel(LocationType type)
        {
            return type switch
            {
                LocationType.Province => "tỉnh",
                LocationType.District => "huyện",
                LocationType.Ward => "xã",
                LocationType.Landmark => "điểm",
                LocationType.Depot => "kho",
                _ => "khu vực"
            };
        }

        private static string BuildLikePattern(string keyword, bool searchAnywhere)
        {
            var escapedKeyword = keyword
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]");

            return searchAnywhere
                ? $"%{escapedKeyword}%"
                : $"{escapedKeyword}%";
        }
    }
}
