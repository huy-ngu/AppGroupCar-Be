using AppGroupCar.Entities;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.RouteRequest
{
    public class CreateBusRoutePointRequest
    {
        [Required]
        public Guid LocationId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sequence must be greater than 0")]
        public int Sequence { get; set; }

        [Range((int)RouteStopType.Start, (int)RouteStopType.End, ErrorMessage = "StopType is invalid")]
        public RouteStopType StopType { get; set; }
    }
}
