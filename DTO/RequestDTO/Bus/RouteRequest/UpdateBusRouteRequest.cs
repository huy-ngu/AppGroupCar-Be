namespace AppGroupCar.DTO.RequestDTO.Bus.RouteRequest
{
    public class UpdateBusRouteRequest
    {
        public string? Name { get; set; }

        public List<CreateBusRoutePointRequest> RoutePoints { get; set; }
            = new();
    }
}
