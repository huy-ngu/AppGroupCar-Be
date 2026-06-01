using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market.Offer
{
    public class GetOfferSharedRideDTO
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public string DriverName { get; set; } = null!;
        public Guid VehicleId { get; set; }
        public string VehicleName { get; set; } = null!;
        public string VehicleUrlImage { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public decimal BasePrice { get; set; }
        public OfferStatus Status { get; set; }
        public LocationDetailDTO StartPoint { get; set; } = null!;
        public LocationDetailDTO EndPoint { get; set; } = null!;
        public int AvailableSeats { get; set; }
    }

    public class LocationDetailDTO
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = null!;
    }
}
