using AppGroupCar.DTO.ResponseDTO.Bus.RouteResponse;
using AppGroupCar.Entities.Bus;

namespace AppGroupCar.DTO.ResponseDTO.Bus.ShowtimeResponse
{
    public class BusShowtimeDetailResponseDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public DateOnly DepartureDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public decimal Price { get; set; }
        public BusShowtimeStatus Status { get; set; }
        public int SeatCount { get; set; }
        public BusRouteResponse Route { get; set; } = new();
        public BusShowtimeVehicleDto Vehicle { get; set; } = new();
        public BusShowtimeDriverDto Driver { get; set; } = new();
    }

    public class BusShowtimeVehicleDto
    {
        public Guid CompanyVehicleId { get; set; }
        public Guid SeatLayoutId { get; set; }
        public string? SeatLayoutName { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public int SeatCapacity { get; set; }
        public int VehicleType { get; set; }
        public string? UrlImage { get; set; }
    }

    public class BusShowtimeDriverDto
    {
        public Guid CompanyDriverId { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string LicenseClass { get; set; } = string.Empty;
    }
}
