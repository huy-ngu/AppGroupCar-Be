using AppGroupCar.Entities.Bus;

namespace AppGroupCar.DTO.ResponseDTO.Bus.ShowtimeResponse
{
    public class BusShowtimeResponse
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid CompanyVehicleId { get; set; }
        public string PlateNumber { get; set; } = null!;
        public Guid RouteId { get; set; }
        public DateOnly DepartureDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public decimal Price { get; set; }
        public BusShowtimeStatus Status { get; set; }
        public int SeatCount { get; set; }
    }
}
