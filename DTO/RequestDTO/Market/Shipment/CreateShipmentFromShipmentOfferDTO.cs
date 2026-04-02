namespace AppGroupCar.DTO.RequestDTO.Market.Shipment
{
    public class CreateShipmentFromShipmentOfferDTO
    {
        public Guid ShipmentOfferId { get; set; }

        public Guid ShipmentRequestId { get; set; }
    }
}
