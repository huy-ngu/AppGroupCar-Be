using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.ShowtimeRequest
{
    public class CreateBusShowtimeRequest
    {
        public Guid CompanyVehicleId { get; set; }
        public Guid RouteId { get; set; }
        public DateOnly DepartureDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
