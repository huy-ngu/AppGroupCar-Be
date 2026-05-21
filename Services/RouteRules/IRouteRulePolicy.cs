using AppGroupCar.Entities;

namespace AppGroupCar.Services.RouteRules
{
    public interface IRouteRulePolicy
    {
        IReadOnlyCollection<LocationType> RouteCreationLocationTypes { get; }

        IReadOnlyCollection<LocationType> BusStartEndLocationTypes { get; }

        IReadOnlyCollection<RouteStopType> PickupStopTypes { get; }

        IReadOnlyCollection<RouteStopType> DropoffStopTypes { get; }

        bool IsRouteLocationTypeAllowed(LocationType locationType);

        bool IsBusStartEndLocationType(LocationType locationType);

        bool IsPickupAllowed(RouteStopType stopType);

        bool IsDropoffAllowed(RouteStopType stopType);
    }
}
