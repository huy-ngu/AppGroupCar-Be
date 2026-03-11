using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market
{
    public class GetOfferDTO
    {
        public Guid Id { get; set; }
        public string DriverName { get; set; } = null!;
        public string VehicleName { get; set; } = null!;
        public ServiceType ServiceType { get; set; }
        public DateTime DepartureTime { get; set; }
        public decimal BasePrice { get; set; }
        public OfferStatus Status { get; set; }
        public int? TotalSeats { get; set; }
        public int? AvailableSeats { get; set; }
    }
}
