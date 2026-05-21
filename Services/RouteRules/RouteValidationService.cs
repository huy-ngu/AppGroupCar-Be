using AppGroupCar.Data;
using AppGroupCar.Entities;
using AppGroupCar.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.RouteRules
{
    public class RouteValidationService : IRouteValidationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRouteRulePolicy _routeRulePolicy;

        public RouteValidationService(
            ApplicationDbContext context,
            IRouteRulePolicy routeRulePolicy)
        {
            _context = context;
            _routeRulePolicy = routeRulePolicy;
        }

        public async Task ValidateRouteForCreationAsync(
            RouteValidationRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request.Stops == null || request.Stops.Count < 2)
            {
                throw new AppException(400, "Route must have at least 2 stops.");
            }

            var orderedStops = request.Stops
                .OrderBy(x => x.Sequence)
                .ToList();

            ValidateStopSequence(orderedStops);
            ValidateStopRoles(orderedStops);

            var locationLookup = await LoadRouteLocationsAsync(orderedStops, cancellationToken);

            foreach (var stop in orderedStops)
            {
                var location = locationLookup[stop.LocationId];

                if (!_routeRulePolicy.IsRouteLocationTypeAllowed(location.LocationType))
                {
                    throw new AppException(
                        400,
                        $"Location type '{location.LocationType}' is not allowed for route stops.");
                }
            }

            if (request.TransportMode == RouteTransportMode.Bus)
            {
                var startStop = orderedStops[0];
                var endStop = orderedStops[^1];

                var startLocation = locationLookup[startStop.LocationId];
                var endLocation = locationLookup[endStop.LocationId];

                if (!_routeRulePolicy.IsBusStartEndLocationType(startLocation.LocationType))
                {
                    throw new AppException(
                        400,
                        "Bus route start location must be a BusStation.");
                }

                if (!_routeRulePolicy.IsBusStartEndLocationType(endLocation.LocationType))
                {
                    throw new AppException(
                        400,
                        "Bus route end location must be a BusStation.");
                }
            }
        }

        public async Task EnsureLocationAllowedForRouteAsync(
            Guid locationId,
            CancellationToken cancellationToken = default)
        {
            var location = await GetRouteLocationOrThrowAsync(locationId, cancellationToken);

            if (!_routeRulePolicy.IsRouteLocationTypeAllowed(location.LocationType))
            {
                throw new AppException(
                    400,
                    $"Location type '{location.LocationType}' is not allowed for route stops.");
            }
        }

        public async Task<Location> GetRouteLocationOrThrowAsync(
            Guid locationId,
            CancellationToken cancellationToken = default)
        {
            if (locationId == Guid.Empty)
            {
                throw new AppException(400, "LocationId is required.");
            }

            var location = await _context.Locations
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == locationId && x.DeleteAt == null && x.IsActive,
                    cancellationToken);

            if (location == null)
            {
                throw new AppException(404, $"Location '{locationId}' was not found or inactive.");
            }

            return location;
        }

        private static void ValidateStopSequence(IReadOnlyList<RouteStopInput> orderedStops)
        {
            if (orderedStops.Any(x => x.LocationId == Guid.Empty))
            {
                throw new AppException(400, "All route stops must have a valid LocationId.");
            }

            if (orderedStops.Any(x => x.Sequence <= 0))
            {
                throw new AppException(400, "Route stop sequence must be greater than 0.");
            }

            var duplicateSequence = orderedStops
                .GroupBy(x => x.Sequence)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicateSequence != null)
            {
                throw new AppException(400, $"Route stop sequence '{duplicateSequence.Key}' is duplicated.");
            }

            for (var i = 0; i < orderedStops.Count; i++)
            {
                var expectedSequence = i + 1;
                if (orderedStops[i].Sequence != expectedSequence)
                {
                    throw new AppException(400, "Route sequence must start at 1 and be continuous.");
                }
            }
        }

        private static void ValidateStopRoles(IReadOnlyList<RouteStopInput> orderedStops)
        {
            var startCount = orderedStops.Count(x => x.StopType == RouteStopType.Start);
            var endCount = orderedStops.Count(x => x.StopType == RouteStopType.End);

            if (startCount != 1 || endCount != 1)
            {
                throw new AppException(400, "Route must contain exactly one Start and one End stop.");
            }

            if (orderedStops[0].StopType != RouteStopType.Start)
            {
                throw new AppException(400, "The first route stop must have StopType.Start.");
            }

            if (orderedStops[^1].StopType != RouteStopType.End)
            {
                throw new AppException(400, "The last route stop must have StopType.End.");
            }

            var containsStartEndInside = orderedStops
                .Skip(1)
                .Take(Math.Max(0, orderedStops.Count - 2))
                .Any(x => x.StopType is RouteStopType.Start or RouteStopType.End);

            if (containsStartEndInside)
            {
                throw new AppException(400, "Only the first stop can be Start and only the last stop can be End.");
            }
        }

        private async Task<Dictionary<Guid, Location>> LoadRouteLocationsAsync(
            IReadOnlyCollection<RouteStopInput> stops,
            CancellationToken cancellationToken)
        {
            var locationIds = stops
                .Select(x => x.LocationId)
                .Distinct()
                .ToList();

            var locations = await _context.Locations
                .AsNoTracking()
                .Where(x =>
                    locationIds.Contains(x.Id) &&
                    x.DeleteAt == null &&
                    x.IsActive)
                .ToListAsync(cancellationToken);

            var locationLookup = locations.ToDictionary(x => x.Id);

            if (locationLookup.Count != locationIds.Count)
            {
                var missingIds = locationIds
                    .Where(id => !locationLookup.ContainsKey(id))
                    .Select(x => x.ToString())
                    .ToList();

                throw new AppException(400, $"Some route locations are invalid or inactive: {string.Join(", ", missingIds)}");
            }

            return locationLookup;
        }
    }
}
