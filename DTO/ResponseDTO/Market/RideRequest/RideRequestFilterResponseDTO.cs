using AppGroupCar.DTO.ResponseDTO.Shared;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market.RideRequest
{
    public class RideRequestFilterResponseDTO
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public CustomerSummaryDTO Customer { get; set; } = null!;
        public LocationSummaryDTO FromLocation { get; set; } = null!;
        public LocationSummaryDTO ToLocation { get; set; } = null!;
        public DateTime DesiredDepartureTime { get; set; }
        public int SeatsRequired { get; set; }
        public RideRequestStatus Status { get; set; }
    }
}
