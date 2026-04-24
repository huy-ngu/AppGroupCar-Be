using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO
{
    public class GetShipmentDetailDTO
    {
        public ShipmentInfoDTO Shipment { get; set; } = null!;
        public ShipmentOfferInfoDTO ShipmentOffer { get; set; } = null!;
        public ShipmentRequestInfoDTO ShipmentRequest { get; set; } = null!;
    }

    public class ShipmentInfoDTO
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public Guid? ShipmentOfferId { get; set; }
        public ShipmentStatus ShipmentStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }

    public class ShipmentOfferInfoDTO
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public Guid RequestId { get; set; }
        public Guid? OfferId { get; set; }
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
        public decimal ProposedPrice { get; set; }
        public ShipmentOfferStatus Status { get; set; }
    }

    public class ShipmentRequestInfoDTO
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }
        public Guid CustomerId { get; set; }
        public Guid PickupLocationId { get; set; }
        public Guid DropoffLocationId { get; set; }
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
