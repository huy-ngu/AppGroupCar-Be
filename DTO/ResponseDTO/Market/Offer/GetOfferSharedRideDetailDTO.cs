using AppGroupCar.Entities;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market.Offer
{
    public class GetOfferSharedRideDetailDTO
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
        public ServiceType ServiceType { get; set; }
        public DateTime DepartureTime { get; set; }
        public int? EstimatedDurationMinutes { get; set; }
        public decimal BasePrice { get; set; }
        public OfferStatus Status { get; set; }
        public SharedRideDetailInfoDTO SharedRideDetail { get; set; } = null!;
        public SharedRideDriverInfoDTO Driver { get; set; } = null!;
        public SharedRideVehicleInfoDTO Vehicle { get; set; } = null!;
        public IEnumerable<SharedRideRoutePointInfoDTO> RoutePoints { get; set; } = [];
    }

    public class SharedRideDetailInfoDTO
    {
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
    }

    public class SharedRideDriverInfoDTO
    {
        public Guid DriverId { get; set; }
        public string Name { get; set; } = null!;
        public string LicenseNumber { get; set; } = null!;
        public string LicenseClass { get; set; } = null!;
        public decimal RatingAverage { get; set; }
        public int RatingCount { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class SharedRideVehicleInfoDTO
    {
        public Guid VehicleId { get; set; }
        public string PlateNumber { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public int SeatCapacity { get; set; }
        public VehicleType VehicleType { get; set; }
        public VehicleStatus Status { get; set; }
        public string? UrlImage { get; set; }
    }

    public class SharedRideRoutePointInfoDTO
    {
        public Guid RoutePointId { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public string? Address { get; set; }
        public LocationType LocationType { get; set; }
        public RouteStopType StopType { get; set; }
        public int Sequence { get; set; }
    }
}
