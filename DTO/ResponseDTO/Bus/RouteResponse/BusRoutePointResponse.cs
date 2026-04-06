namespace AppGroupCar.DTO.ResponseDTO.Bus.RouteResponse
{
    public class BusRoutePointResponse
    {
        public Guid Id { get; set; }

        public Guid LocationId { get; set; }

        public string LocationName { get; set; } = "";

        public int Sequence { get; set; }

        public bool PickupAllowed { get; set; }

        public bool DropoffAllowed { get; set; }
    }
}
