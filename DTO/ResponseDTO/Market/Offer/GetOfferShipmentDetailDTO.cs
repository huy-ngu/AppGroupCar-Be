using AppGroupCar.Entities;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market.Offer
{
    public class GetOfferShipmentDetailDTO
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
        public ServiceType ServiceType { get; set; }
        public DateTime DepartureTime { get; set; }
        public int? EstimatedDurationMinutes { get; set; }
        public decimal BasePrice { get; set; }
        public OfferStatus Status { get; set; }
        public CargoDetailDTO CargoDetail { get; set; } = null!;
        public SharedRideDriverInfoDTO Driver { get; set; } = null!;
        public SharedRideVehicleInfoDTO Vehicle { get; set; } = null!;
        public IEnumerable<SharedRideRoutePointInfoDTO> RoutePoints { get; set; } = [];
    }
}
