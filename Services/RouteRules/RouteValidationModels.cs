using AppGroupCar.Entities;

namespace AppGroupCar.Services.RouteRules
{
    public enum RouteTransportMode
    {
        Bus = 1,
        SharedRide = 2,
        Truck = 3
    }

    public sealed record RouteStopInput(
        Guid LocationId,
        int Sequence,
        RouteStopType StopType);

    public sealed record RouteValidationRequest(
        RouteTransportMode TransportMode,
        IReadOnlyCollection<RouteStopInput> Stops);
}
