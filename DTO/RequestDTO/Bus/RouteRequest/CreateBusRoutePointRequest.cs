namespace AppGroupCar.DTO.RequestDTO.Bus.RouteRequest
{
    public class CreateBusRoutePointRequest
    {
        public Guid LocationId { get; set; }

        public int Sequence { get; set; }

        public bool PickupAllowed { get; set; }

        public bool DropoffAllowed { get; set; }
    }
}
