using AppGroupCar.DTO.ResponseDTO.Shared;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO
{
    public class ShipmentRequestFilterResponseDTO
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public CustomerSummaryDTO Customer { get; set; } = null!;
        public LocationSummaryDTO PickupLocation { get; set; } = null!;
        public LocationSummaryDTO DropoffLocation { get; set; } = null!;
        public DateTime DeliveryDate { get; set; }
        public decimal Weight { get; set; }
        public decimal Volume { get; set; }
        public string? Description { get; set; }
        public bool IsFragile { get; set; }
        public string? HandlingNote { get; set; }
        public ShipmentRequestStatus Status { get; set; }
        public string? ImageUrl { get; set; }
    }
}
