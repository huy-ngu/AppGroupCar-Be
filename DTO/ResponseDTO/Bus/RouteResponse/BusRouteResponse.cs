namespace AppGroupCar.DTO.ResponseDTO.Bus.RouteResponse
{
    public class BusRouteResponse
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public Guid CompanyId { get; set; }

        public List<BusRoutePointResponse> RoutePoints { get; set; }
            = new();
    }
}
