using AppGroupCar.Entities.Bus;

namespace AppGroupCar.DTO.ResponseDTO.Bus.ShowtimeResponse
{
    public class UpcomingShowtimeListResponseDto
    {
        public Guid CompanyId { get; set; }
        public DateOnly FromDate { get; set; }
        public int Total { get; set; }
        public List<UpcomingShowtimeItemDto> Items { get; set; } = new();
    }

    public class UpcomingShowtimeItemDto
    {
        public Guid Id { get; set; }
        public Guid RouteId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public Guid CompanyVehicleId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public Guid CompanyDriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public DateOnly DepartureDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public decimal Price { get; set; }
        public BusShowtimeStatus Status { get; set; }
        public int SeatCount { get; set; }
    }
}
