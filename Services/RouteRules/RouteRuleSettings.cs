namespace AppGroupCar.Services.RouteRules
{
    public class RouteRuleSettings
    {
        public List<int> AllowedLocationTypesForRouteCreation { get; set; } = new() { 1, 2, 3, 4, 5, 6 };

        public List<int> BusStartEndLocationTypes { get; set; } = new() { 4 };

        public List<int> PickupStopTypes { get; set; } = new() { 1 };

        public List<int> DropoffStopTypes { get; set; } = new() { 4, 5 };
    }
}
