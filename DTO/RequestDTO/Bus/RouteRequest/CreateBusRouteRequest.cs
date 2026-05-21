using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.RouteRequest
{
    public class CreateBusRouteRequest
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [Required]
        [MinLength(2)]
        public List<CreateBusRoutePointRequest> RoutePoints { get; set; } = new();
    }
}
