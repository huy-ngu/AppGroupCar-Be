using AppGroupCar.Entities;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.OfferRoutePoint
{
    public class UpdateOfferRoutePointRequestDTO
    {
        public Guid? OfferId { get; set; }

        public Guid? LocationId { get; set; }

        [Range((int)RouteStopType.Start, (int)RouteStopType.End, ErrorMessage = "StopType is invalid")]
        public RouteStopType? StopType { get; set; }
    }
}
