using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.RouteRequest
{
    public class CreateBusRouteRequest
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [Range(1, 10080, ErrorMessage = "EstimatedDurationMinutes phai lon hon 0 va nho hon 10080")]
        public int EstimatedDurationMinutes { get; set; }

        [Required]
        [MinLength(2)]
        public List<CreateBusRoutePointRequest> RoutePoints { get; set; } = new();
    }
}
