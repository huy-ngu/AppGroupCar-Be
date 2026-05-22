using AppGroupCar.Entities;
using Microsoft.Extensions.Options;

namespace AppGroupCar.Services.RouteRules
{
    public class RouteRulePolicy : IRouteRulePolicy
    {
        private readonly HashSet<LocationType> _routeCreationLocationTypes;
        private readonly HashSet<LocationType> _busStartEndLocationTypes;
        private readonly HashSet<RouteStopType> _pickupStopTypes;
        private readonly HashSet<RouteStopType> _dropoffStopTypes;

        public RouteRulePolicy(IOptions<RouteRuleSettings> options)
        {
            var settings = options.Value ?? new RouteRuleSettings();

            _routeCreationLocationTypes = ParseEnumSet<LocationType>(
                settings.AllowedLocationTypesForRouteCreation,
                nameof(RouteRuleSettings.AllowedLocationTypesForRouteCreation));

            _busStartEndLocationTypes = ParseEnumSet<LocationType>(
                settings.BusStartEndLocationTypes,
                nameof(RouteRuleSettings.BusStartEndLocationTypes));

            _pickupStopTypes = ParseEnumSet<RouteStopType>(
                settings.PickupStopTypes,
                nameof(RouteRuleSettings.PickupStopTypes));

            _dropoffStopTypes = ParseEnumSet<RouteStopType>(
                settings.DropoffStopTypes,
                nameof(RouteRuleSettings.DropoffStopTypes));
        }

        public IReadOnlyCollection<LocationType> RouteCreationLocationTypes => _routeCreationLocationTypes;

        public IReadOnlyCollection<LocationType> BusStartEndLocationTypes => _busStartEndLocationTypes;

        public IReadOnlyCollection<RouteStopType> PickupStopTypes => _pickupStopTypes;

        public IReadOnlyCollection<RouteStopType> DropoffStopTypes => _dropoffStopTypes;

        public bool IsRouteLocationTypeAllowed(LocationType locationType)
        {
            return _routeCreationLocationTypes.Contains(locationType);
        }

        public bool IsBusStartEndLocationType(LocationType locationType)
        {
            return _busStartEndLocationTypes.Contains(locationType);
        }

        public bool IsPickupAllowed(RouteStopType stopType)
        {
            return _pickupStopTypes.Contains(stopType);
        }

        public bool IsDropoffAllowed(RouteStopType stopType)
        {
            return _dropoffStopTypes.Contains(stopType);
        }

        private static HashSet<TEnum> ParseEnumSet<TEnum>(
            IEnumerable<int>? values,
            string settingName)
            where TEnum : struct, Enum
        {
            if (values == null)
            {
                throw new InvalidOperationException($"{settingName} must not be null.");
            }

            var parsedValues = new HashSet<TEnum>();

            foreach (var value in values.Distinct())
            {
                if (!Enum.IsDefined(typeof(TEnum), value))
                {
                    throw new InvalidOperationException($"{settingName} contains invalid enum value: {value}");
                }

                parsedValues.Add((TEnum)Enum.ToObject(typeof(TEnum), value));
            }

            if (parsedValues.Count == 0)
            {
                throw new InvalidOperationException($"{settingName} must contain at least one value.");
            }

            return parsedValues;
        }
    }
}
