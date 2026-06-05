using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO
{
    public class GetPendingShipmentOfferDTO
    {
        public Guid Id { get; set; }
        public Guid? OfferId { get; set; }
        public Guid RequestId { get; set; }
        public decimal ProposedPrice { get; set; }
        public ShipmentOfferStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public ShipmentRequestInfoDTO ShipmentRequest { get; set; } = null!;
    }
}
