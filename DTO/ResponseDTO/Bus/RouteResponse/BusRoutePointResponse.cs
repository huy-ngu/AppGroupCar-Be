using AppGroupCar.Entities;

namespace AppGroupCar.DTO.ResponseDTO.Bus.RouteResponse
{
    public class BusRoutePointResponse
    {
        public Guid Id { get; set; }

        public Guid LocationId { get; set; }

        public string LocationName { get; set; } = "";

        public int Sequence { get; set; }

        public RouteStopType StopType { get; set; }

        public bool PickupAllowed => StopType is RouteStopType.Start or RouteStopType.Pickup;

        public bool DropoffAllowed => StopType is RouteStopType.Dropoff or RouteStopType.End;
    }
}
