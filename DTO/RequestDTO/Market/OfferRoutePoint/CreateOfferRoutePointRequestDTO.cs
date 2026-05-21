using AppGroupCar.Entities;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.OfferRoutePoint
{
    public class CreateOfferRoutePointRequestDTO
    {
        [Required]
        public Guid OfferId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sequence must be greater than 0")]
        public int Sequence { get; set; }

        [Required]
        public Guid LocationId { get; set; }

        [Range((int)RouteStopType.Start, (int)RouteStopType.End, ErrorMessage = "StopType is invalid")]
        public RouteStopType StopType { get; set; }
    }
}
