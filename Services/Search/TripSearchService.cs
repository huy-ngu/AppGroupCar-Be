using AppGroupCar.Data;
using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.ResponseDTO.Search;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Entities.Market;
using AppGroupCar.Services.RouteRules;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Search
{
    public class TripSearchService : ITripSearchService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRouteRulePolicy _routeRulePolicy;

        private sealed class LocationSearchSet
        {
            public LocationSearchSet(
                Guid requestedLocationId,
                LocationType requestedLocationType,
                Guid? parentId,
                List<Guid> exactRouteLocationIds,
                List<Guid> siblingRouteLocationIds,
                List<Guid> nearbyRouteLocationIds)
            {
                RequestedLocationId = requestedLocationId;
                RequestedLocationType = requestedLocationType;
                ParentId = parentId;
                ExactRouteLocationIds = exactRouteLocationIds;
                SiblingRouteLocationIds = siblingRouteLocationIds;
                NearbyRouteLocationIds = nearbyRouteLocationIds;
                ExactRouteLocationIdSet = exactRouteLocationIds.ToHashSet();
                SiblingRouteLocationIdSet = siblingRouteLocationIds.ToHashSet();
            }

            public Guid RequestedLocationId { get; }
            public LocationType RequestedLocationType { get; }
            public Guid? ParentId { get; }
            public List<Guid> ExactRouteLocationIds { get; }
            public List<Guid> SiblingRouteLocationIds { get; }
            public List<Guid> NearbyRouteLocationIds { get; }
            public HashSet<Guid> ExactRouteLocationIdSet { get; }
            public HashSet<Guid> SiblingRouteLocationIdSet { get; }
        }

        private sealed record RouteFilterCandidateSet(
            List<Guid>? PickupIds,
            List<Guid>? DropoffIds,
            bool PickupAllowNearby,
            bool DropoffAllowNearby);

        private sealed record LocationMatchResult(
            TripLocationMatchType MatchType,
            Guid? MatchedLocationId,
            int Score,
            bool UsedNearby);

        private sealed record TripSearchResult(TripSearchItemDTO Item, int Score);

        public TripSearchService(
            ApplicationDbContext context,
            IRouteRulePolicy routeRulePolicy)
        {
            _context = context;
            _routeRulePolicy = routeRulePolicy;
        }

        public async Task<PagedResult<TripSearchItemDTO>> SearchTripsAsync(TripSearchParameters parameters)
        {
            var selectedServices = ResolveSelectedTripTypes(parameters);
            if (selectedServices.Count == 0)
            {
                return CreateEmptyResult(parameters);
            }

            var locationSets = await ResolveLocationSearchSetsAsync(
                parameters.PickupLocationId,
                parameters.DropoffLocationId);

            locationSets.TryGetValue(parameters.PickupLocationId ?? Guid.Empty, out var pickupSet);
            locationSets.TryGetValue(parameters.DropoffLocationId ?? Guid.Empty, out var dropoffSet);

            if ((parameters.PickupLocationId.HasValue && pickupSet == null) ||
                (parameters.DropoffLocationId.HasValue && dropoffSet == null))
            {
                return CreateEmptyResult(parameters);
            }

            if ((pickupSet != null && pickupSet.ExactRouteLocationIds.Count == 0) ||
                (dropoffSet != null && dropoffSet.ExactRouteLocationIds.Count == 0))
            {
                return CreateEmptyResult(parameters);
            }

            var (departureDate, departureTime) = ResolveDepartureFilter(parameters);
            var routeCandidates = BuildRouteCandidates(parameters, pickupSet, dropoffSet);

            var trips = await LoadTripsBySourcesAsync(
                selectedServices,
                departureDate,
                departureTime,
                pickupSet,
                dropoffSet,
                routeCandidates);

            var orderedTrips = trips
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Item.DepartureTime)
                .ToList();

            var pageItems = orderedTrips
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(x => x.Item)
                .ToList();

            return new PagedResult<TripSearchItemDTO>
            {
                Items = pageItems,
                TotalCount = orderedTrips.Count,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }

        private async Task<List<TripSearchResult>> LoadTripsBySourcesAsync(
            IReadOnlyCollection<TripType> selectedServices,
            DateOnly? departureDate,
            TimeOnly? departureTime,
            LocationSearchSet? pickupSet,
            LocationSearchSet? dropoffSet,
            RouteFilterCandidateSet candidates)
        {
            var results = new List<TripSearchResult>();

            if (selectedServices.Contains(TripType.Bus))
            {
                var busResults = await LoadBusTripsAsync(
                    departureDate,
                    departureTime,
                    pickupSet,
                    dropoffSet,
                    candidates);
                results.AddRange(busResults);
            }

            if (selectedServices.Contains(TripType.SharedRide))
            {
                var sharedResults = await LoadOfferTripsAsync(
                    ServiceType.Shared,
                    TripType.SharedRide,
                    departureDate,
                    departureTime,
                    pickupSet,
                    dropoffSet,
                    candidates);
                results.AddRange(sharedResults);
            }

            if (selectedServices.Contains(TripType.Truck))
            {
                var truckResults = await LoadOfferTripsAsync(
                    ServiceType.Truck,
                    TripType.Truck,
                    departureDate,
                    departureTime,
                    pickupSet,
                    dropoffSet,
                    candidates);
                results.AddRange(truckResults);
            }

            return results;
        }

        private async Task<List<TripSearchResult>> LoadBusTripsAsync(
            DateOnly? departureDate,
            TimeOnly? departureTime,
            LocationSearchSet? pickupSet,
            LocationSearchSet? dropoffSet,
            RouteFilterCandidateSet candidates)
        {
            var pickupStopTypes = _routeRulePolicy.PickupStopTypes.ToList();
            var dropoffStopTypes = _routeRulePolicy.DropoffStopTypes.ToList();

            var query = _context.BusShowtimes
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.Route)
                    .ThenInclude(r => r.RoutePoints)
                        .ThenInclude(rp => rp.Location)
                .Where(x => x.DeleteAt == null && x.Route.DeleteAt == null)
                .Where(x => x.Status == BusShowtimeStatus.Active || x.Status == BusShowtimeStatus.Scheduled);

            query = ApplyBusTimeFilter(query, departureDate, departureTime);
            query = ApplyBusRouteFilter(query, candidates.PickupIds, candidates.DropoffIds, pickupStopTypes, dropoffStopTypes);

            var showtimes = await query.ToListAsync();

            return showtimes.Select(showtime =>
            {
                var routePoints = showtime.Route.RoutePoints
                    .Where(rp => rp.DeleteAt == null)
                    .OrderBy(rp => rp.Sequence)
                    .Select(rp => new TripRoutePointDTO
                    {
                        LocationId = rp.LocationId,
                        LocationName = rp.Location?.Name ?? string.Empty,
                        Sequence = rp.Sequence,
                        StopType = rp.StopType
                    })
                    .ToList();

                var match = BuildMatch(routePoints, pickupSet, dropoffSet, candidates);

                var item = new TripSearchItemDTO
                {
                    TripId = showtime.Id,
                    TripType = TripType.Bus,
                    ProviderName = showtime.Company.CompanyName,
                    DepartureTime = showtime.DepartureDate.ToDateTime(showtime.DepartureTime),
                    Price = showtime.Price,
                    RoutePoints = routePoints,
                    Match = match,
                    Reference = BuildReference(showtime.Id, TripType.Bus)
                };

                return new TripSearchResult(item, GetScoreFromMatch(match));
            }).ToList();
        }

        private async Task<List<TripSearchResult>> LoadOfferTripsAsync(
            ServiceType serviceType,
            TripType tripType,
            DateOnly? departureDate,
            TimeOnly? departureTime,
            LocationSearchSet? pickupSet,
            LocationSearchSet? dropoffSet,
            RouteFilterCandidateSet candidates)
        {
            var pickupStopTypes = _routeRulePolicy.PickupStopTypes.ToList();
            var dropoffStopTypes = _routeRulePolicy.DropoffStopTypes.ToList();

            var query = _context.Offers
                .AsNoTracking()
                .Include(o => o.Driver)
                .Include(o => o.RoutePoints)
                .Where(o =>
                    o.DeleteAt == null &&
                    o.Status == OfferStatus.Active &&
                    o.ServiceType == serviceType);

            query = ApplyOfferTimeFilter(query, departureDate, departureTime);
            query = ApplyOfferRouteFilter(query, candidates.PickupIds, candidates.DropoffIds, pickupStopTypes, dropoffStopTypes);

            var offers = await query.ToListAsync();

            var locationIds = offers
                .SelectMany(o => o.RoutePoints)
                .Where(rp => rp.DeleteAt == null)
                .Select(rp => rp.LocationId)
                .Distinct()
                .ToList();

            var locationLookup = locationIds.Count == 0
                ? new Dictionary<Guid, string>()
                : await _context.Locations
                    .AsNoTracking()
                    .Where(x => locationIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.Name);

            return offers.Select(offer =>
            {
                var routePoints = offer.RoutePoints
                    .Where(rp => rp.DeleteAt == null)
                    .OrderBy(rp => rp.Sequence)
                    .Select(rp => new TripRoutePointDTO
                    {
                        LocationId = rp.LocationId,
                        LocationName = locationLookup.TryGetValue(rp.LocationId, out var name)
                            ? name
                            : string.Empty,
                        Sequence = rp.Sequence,
                        StopType = rp.StopType
                    })
                    .ToList();

                var match = BuildMatch(routePoints, pickupSet, dropoffSet, candidates);

                var item = new TripSearchItemDTO
                {
                    TripId = offer.Id,
                    TripType = tripType,
                    ProviderName = offer.Driver.Name,
                    DepartureTime = offer.DepartureTime,
                    Price = offer.BasePrice,
                    RoutePoints = routePoints,
                    Match = match,
                    Reference = BuildReference(offer.Id, tripType)
                };

                return new TripSearchResult(item, GetScoreFromMatch(match));
            }).ToList();
        }

        private static IQueryable<BusShowtime> ApplyBusRouteFilter(
            IQueryable<BusShowtime> query,
            List<Guid>? pickupIds,
            List<Guid>? dropoffIds,
            List<RouteStopType> pickupStopTypes,
            List<RouteStopType> dropoffStopTypes)
        {
            if (pickupIds != null && pickupIds.Count == 0)
            {
                return query.Where(_ => false);
            }

            if (dropoffIds != null && dropoffIds.Count == 0)
            {
                return query.Where(_ => false);
            }

            if (pickupIds != null && pickupIds.Count > 0 && dropoffIds != null && dropoffIds.Count > 0)
            {
                query = query.Where(showtime =>
                    showtime.Route.RoutePoints.Any(pickup =>
                        pickup.DeleteAt == null &&
                        pickupStopTypes.Contains(pickup.StopType) &&
                        pickupIds.Contains(pickup.LocationId) &&
                        showtime.Route.RoutePoints.Any(dropoff =>
                            dropoff.DeleteAt == null &&
                            dropoffStopTypes.Contains(dropoff.StopType) &&
                            dropoffIds.Contains(dropoff.LocationId) &&
                            pickup.Sequence < dropoff.Sequence)));
            }
            else if (pickupIds != null && pickupIds.Count > 0)
            {
                query = query.Where(showtime =>
                    showtime.Route.RoutePoints.Any(rp =>
                        rp.DeleteAt == null &&
                        pickupStopTypes.Contains(rp.StopType) &&
                        pickupIds.Contains(rp.LocationId)));
            }
            else if (dropoffIds != null && dropoffIds.Count > 0)
            {
                query = query.Where(showtime =>
                    showtime.Route.RoutePoints.Any(rp =>
                        rp.DeleteAt == null &&
                        dropoffStopTypes.Contains(rp.StopType) &&
                        dropoffIds.Contains(rp.LocationId)));
            }

            return query;
        }

        private static IQueryable<Offer> ApplyOfferRouteFilter(
            IQueryable<Offer> query,
            List<Guid>? pickupIds,
            List<Guid>? dropoffIds,
            List<RouteStopType> pickupStopTypes,
            List<RouteStopType> dropoffStopTypes)
        {
            if (pickupIds != null && pickupIds.Count == 0)
            {
                return query.Where(_ => false);
            }

            if (dropoffIds != null && dropoffIds.Count == 0)
            {
                return query.Where(_ => false);
            }

            if (pickupIds != null && pickupIds.Count > 0 && dropoffIds != null && dropoffIds.Count > 0)
            {
                query = query.Where(offer =>
                    offer.RoutePoints.Any(pickup =>
                        pickup.DeleteAt == null &&
                        pickupStopTypes.Contains(pickup.StopType) &&
                        pickupIds.Contains(pickup.LocationId) &&
                        offer.RoutePoints.Any(dropoff =>
                            dropoff.DeleteAt == null &&
                            dropoffStopTypes.Contains(dropoff.StopType) &&
                            dropoffIds.Contains(dropoff.LocationId) &&
                            pickup.Sequence < dropoff.Sequence)));
            }
            else if (pickupIds != null && pickupIds.Count > 0)
            {
                query = query.Where(offer =>
                    offer.RoutePoints.Any(rp =>
                        rp.DeleteAt == null &&
                        pickupStopTypes.Contains(rp.StopType) &&
                        pickupIds.Contains(rp.LocationId)));
            }
            else if (dropoffIds != null && dropoffIds.Count > 0)
            {
                query = query.Where(offer =>
                    offer.RoutePoints.Any(rp =>
                        rp.DeleteAt == null &&
                        dropoffStopTypes.Contains(rp.StopType) &&
                        dropoffIds.Contains(rp.LocationId)));
            }

            return query;
        }

        private static IQueryable<BusShowtime> ApplyBusTimeFilter(
            IQueryable<BusShowtime> query,
            DateOnly? departureDate,
            TimeOnly? departureTime)
        {
            if (departureDate.HasValue)
            {
                var date = departureDate.Value;
                query = query.Where(x => x.DepartureDate == date);

                if (departureTime.HasValue)
                {
                    var time = departureTime.Value;
                    query = query.Where(x => x.DepartureTime >= time);
                }

                return query;
            }

            if (departureTime.HasValue)
            {
                var time = departureTime.Value;
                query = query.Where(x => x.DepartureTime >= time);
            }

            return query;
        }

        private static IQueryable<Offer> ApplyOfferTimeFilter(
            IQueryable<Offer> query,
            DateOnly? departureDate,
            TimeOnly? departureTime)
        {
            if (departureDate.HasValue)
            {
                var date = departureDate.Value.ToDateTime(TimeOnly.MinValue).Date;
                query = query.Where(o => o.DepartureTime.Date == date);

                if (departureTime.HasValue)
                {
                    var time = departureTime.Value.ToTimeSpan();
                    query = query.Where(o => o.DepartureTime.TimeOfDay >= time);
                }

                return query;
            }

            if (departureTime.HasValue)
            {
                var time = departureTime.Value.ToTimeSpan();
                query = query.Where(o => o.DepartureTime.TimeOfDay >= time);
            }

            return query;
        }

        private static RouteFilterCandidateSet BuildRouteCandidates(
            TripSearchParameters parameters,
            LocationSearchSet? pickupSet,
            LocationSearchSet? dropoffSet)
        {
            var allowPickupNearby = parameters.EnableNearbySearch && parameters.ExpandPickupLocation;
            var allowDropoffNearby = parameters.EnableNearbySearch && parameters.ExpandDropoffLocation;

            return new RouteFilterCandidateSet(
                BuildCandidateIds(pickupSet, allowPickupNearby),
                BuildCandidateIds(dropoffSet, allowDropoffNearby),
                allowPickupNearby,
                allowDropoffNearby);
        }

        private static List<Guid>? BuildCandidateIds(LocationSearchSet? searchSet, bool allowNearby)
        {
            if (searchSet == null)
            {
                return null;
            }

            return allowNearby
                ? searchSet.NearbyRouteLocationIds
                : searchSet.ExactRouteLocationIds;
        }

        private TripSearchMatchDTO BuildMatch(
            IReadOnlyList<TripRoutePointDTO> routePoints,
            LocationSearchSet? pickupSet,
            LocationSearchSet? dropoffSet,
            RouteFilterCandidateSet candidates)
        {
            var pickup = BuildLocationMatch(routePoints, pickupSet, forPickup: true, candidates.PickupAllowNearby);
            var dropoff = BuildLocationMatch(routePoints, dropoffSet, forPickup: false, candidates.DropoffAllowNearby);

            return new TripSearchMatchDTO
            {
                ExactPickupLocationId = pickupSet?.RequestedLocationId,
                ExactDropoffLocationId = dropoffSet?.RequestedLocationId,
                MatchedPickupLocationId = pickup.MatchedLocationId,
                MatchedDropoffLocationId = dropoff.MatchedLocationId,
                PickupMatchType = pickup.MatchType,
                DropoffMatchType = dropoff.MatchType,
                UsedNearbyForPickup = pickup.UsedNearby,
                UsedNearbyForDropoff = dropoff.UsedNearby
            };
        }

        private LocationMatchResult BuildLocationMatch(
            IReadOnlyList<TripRoutePointDTO> routePoints,
            LocationSearchSet? searchSet,
            bool forPickup,
            bool allowNearby)
        {
            if (searchSet == null)
            {
                return new LocationMatchResult(TripLocationMatchType.None, null, 0, false);
            }

            var exactPoint = routePoints.FirstOrDefault(x =>
                searchSet.ExactRouteLocationIdSet.Contains(x.LocationId) &&
                IsStopUsable(x.StopType, forPickup));
            if (exactPoint != null)
            {
                return new LocationMatchResult(TripLocationMatchType.Exact, exactPoint.LocationId, 3, false);
            }

            if (!allowNearby)
            {
                return new LocationMatchResult(TripLocationMatchType.None, null, 0, false);
            }

            var siblingPoint = routePoints.FirstOrDefault(x =>
                searchSet.SiblingRouteLocationIdSet.Contains(x.LocationId) &&
                IsStopUsable(x.StopType, forPickup));
            if (siblingPoint != null)
            {
                return new LocationMatchResult(TripLocationMatchType.Sibling, siblingPoint.LocationId, 2, true);
            }

            if (searchSet.ParentId.HasValue)
            {
                var parentPoint = routePoints.FirstOrDefault(x =>
                    x.LocationId == searchSet.ParentId.Value &&
                    IsStopUsable(x.StopType, forPickup));
                if (parentPoint != null)
                {
                    return new LocationMatchResult(TripLocationMatchType.Parent, parentPoint.LocationId, 1, true);
                }
            }

            return new LocationMatchResult(TripLocationMatchType.None, null, 0, false);
        }

        private bool IsStopUsable(RouteStopType stopType, bool forPickup)
        {
            return forPickup
                ? _routeRulePolicy.IsPickupAllowed(stopType)
                : _routeRulePolicy.IsDropoffAllowed(stopType);
        }

        private static int GetScoreFromMatch(TripSearchMatchDTO match)
        {
            return GetMatchTypeScore(match.PickupMatchType) + GetMatchTypeScore(match.DropoffMatchType);
        }

        private static int GetMatchTypeScore(TripLocationMatchType matchType)
        {
            return matchType switch
            {
                TripLocationMatchType.Exact => 3,
                TripLocationMatchType.Sibling => 2,
                TripLocationMatchType.Parent => 1,
                _ => 0
            };
        }

        private static TripSearchReferenceDTO BuildReference(Guid id, TripType tripType)
        {
            return new TripSearchReferenceDTO
            {
                Id = id,
                TripType = tripType,
                ServiceCode = tripType switch
                {
                    TripType.Bus => "BUS",
                    TripType.SharedRide => "SHARE_RIDE",
                    TripType.Truck => "SHIPMENT",
                    _ => "UNKNOWN"
                },
                DetailApi = tripType switch
                {
                    TripType.Bus => $"/bus-showtimes/{id}",
                    TripType.SharedRide => $"/Offer/{id}",
                    TripType.Truck => $"/Offer/{id}",
                    _ => string.Empty
                }
            };
        }

        private static (DateOnly? DepartureDate, TimeOnly? DepartureTime) ResolveDepartureFilter(TripSearchParameters parameters)
        {
            var departureDate = parameters.DepartureDate;
            var departureTime = parameters.DepartureTime;

            if (parameters.DepartureFrom.HasValue)
            {
                var departureFrom = parameters.DepartureFrom.Value;

                if (!departureDate.HasValue)
                {
                    departureDate = DateOnly.FromDateTime(departureFrom);
                }

                if (!departureTime.HasValue)
                {
                    departureTime = TimeOnly.FromDateTime(departureFrom);
                }
            }

            return (departureDate, departureTime);
        }

        private static HashSet<TripType> ResolveSelectedTripTypes(TripSearchParameters parameters)
        {
            if (parameters.Services != null && parameters.Services.Count > 0)
            {
                return parameters.Services
                    .Where(x => x is TripType.Bus or TripType.SharedRide or TripType.Truck)
                    .ToHashSet();
            }

            var hasIncludeFlags = parameters.IncludeBus.HasValue ||
                                  parameters.IncludeSharedRide.HasValue ||
                                  parameters.IncludeTruck.HasValue;

            if (!hasIncludeFlags)
            {
                return new HashSet<TripType>
                {
                    TripType.Bus,
                    TripType.SharedRide,
                    TripType.Truck
                };
            }

            var selected = new HashSet<TripType>();
            if (parameters.IncludeBus == true)
            {
                selected.Add(TripType.Bus);
            }

            if (parameters.IncludeSharedRide == true)
            {
                selected.Add(TripType.SharedRide);
            }

            if (parameters.IncludeTruck == true)
            {
                selected.Add(TripType.Truck);
            }

            return selected;
        }

        private static PagedResult<TripSearchItemDTO> CreateEmptyResult(TripSearchParameters parameters)
        {
            return new PagedResult<TripSearchItemDTO>
            {
                Items = new List<TripSearchItemDTO>(),
                TotalCount = 0,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }

        private async Task<Dictionary<Guid, LocationSearchSet>> ResolveLocationSearchSetsAsync(
            Guid? pickupLocationId,
            Guid? dropoffLocationId)
        {
            var targetIds = new[] { pickupLocationId, dropoffLocationId }
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            if (targetIds.Count == 0)
            {
                return new Dictionary<Guid, LocationSearchSet>();
            }

            var locations = await _context.Locations
                .AsNoTracking()
                .Where(x => targetIds.Contains(x.Id) && x.DeleteAt == null && x.IsActive)
                .ToListAsync();

            var locationLookup = locations.ToDictionary(x => x.Id);
            var result = new Dictionary<Guid, LocationSearchSet>();

            foreach (var targetId in targetIds)
            {
                if (!locationLookup.TryGetValue(targetId, out var location))
                {
                    continue;
                }

                var searchSet = await BuildLocationSearchSetAsync(location);
                result[targetId] = searchSet;
            }

            return result;
        }

        private async Task<LocationSearchSet> BuildLocationSearchSetAsync(Location location)
        {
            var allowedTypeCodes = _routeRulePolicy.RouteCreationLocationTypes
                .Select(x => (int)x)
                .ToList();

            if (location.LocationType is LocationType.Province or LocationType.District)
            {
                var descendantRouteLocationIds = await LoadDescendantRouteLocationIdsAsync(location.Id, allowedTypeCodes);
                return new LocationSearchSet(
                    requestedLocationId: location.Id,
                    requestedLocationType: location.LocationType,
                    parentId: null,
                    exactRouteLocationIds: descendantRouteLocationIds,
                    siblingRouteLocationIds: descendantRouteLocationIds,
                    nearbyRouteLocationIds: descendantRouteLocationIds);
            }

            var exactIds = new List<Guid> { location.Id };
            List<Guid> siblingIds;

            if (location.ParentLocationId.HasValue)
            {
                var parentId = location.ParentLocationId.Value;
                siblingIds = await _context.Locations
                    .AsNoTracking()
                    .Where(x =>
                        x.DeleteAt == null &&
                        x.IsActive &&
                        x.ParentLocationId == parentId &&
                        allowedTypeCodes.Contains((int)x.LocationType))
                    .Select(x => x.Id)
                    .ToListAsync();
            }
            else
            {
                siblingIds = new List<Guid> { location.Id };
            }

            if (!siblingIds.Contains(location.Id))
            {
                siblingIds.Add(location.Id);
            }

            var nearbyIds = siblingIds.Distinct().ToList();

            if (location.ParentLocationId.HasValue)
            {
                var parentLocation = await _context.Locations
                    .AsNoTracking()
                    .Where(x =>
                        x.Id == location.ParentLocationId.Value &&
                        x.DeleteAt == null &&
                        x.IsActive)
                    .Select(x => new { x.Id, x.LocationType })
                    .FirstOrDefaultAsync();

                if (parentLocation != null &&
                    allowedTypeCodes.Contains((int)parentLocation.LocationType) &&
                    !nearbyIds.Contains(parentLocation.Id))
                {
                    nearbyIds.Add(parentLocation.Id);
                }
            }

            return new LocationSearchSet(
                requestedLocationId: location.Id,
                requestedLocationType: location.LocationType,
                parentId: location.ParentLocationId,
                exactRouteLocationIds: exactIds,
                siblingRouteLocationIds: siblingIds,
                nearbyRouteLocationIds: nearbyIds);
        }

        private async Task<List<Guid>> LoadDescendantRouteLocationIdsAsync(
            Guid rootLocationId,
            IReadOnlyCollection<int> allowedTypeCodes)
        {
            var matchedIds = new HashSet<Guid>();
            var visitedIds = new HashSet<Guid> { rootLocationId };
            var frontier = new List<Guid> { rootLocationId };

            while (frontier.Count > 0)
            {
                var children = await _context.Locations
                    .AsNoTracking()
                    .Where(x =>
                        x.DeleteAt == null &&
                        x.IsActive &&
                        x.ParentLocationId.HasValue &&
                        frontier.Contains(x.ParentLocationId.Value))
                    .Select(x => new { x.Id, x.LocationType })
                    .ToListAsync();

                var nextFrontier = new List<Guid>();

                foreach (var child in children)
                {
                    if (!visitedIds.Add(child.Id))
                    {
                        continue;
                    }

                    nextFrontier.Add(child.Id);

                    if (allowedTypeCodes.Contains((int)child.LocationType))
                    {
                        matchedIds.Add(child.Id);
                    }
                }

                frontier = nextFrontier;
            }

            return matchedIds.ToList();
        }
    }
}
