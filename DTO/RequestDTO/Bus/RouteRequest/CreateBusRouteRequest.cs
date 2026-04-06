namespace AppGroupCar.DTO.RequestDTO.Bus.RouteRequest
{
    public class CreateBusRouteRequest
    {
        public string? Name { get; set; }

        public List<CreateBusRoutePointRequest> RoutePoints { get; set; }
            = new();
    }
}
