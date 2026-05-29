using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO
{
    public class GetDetailShipmentOfferDTO
    {
        public Guid Id { get; set; }
        public Guid? OfferId { get; set; }
        public Guid RequestId { get; set; }
        public decimal ProposedPrice { get; set; }
        public ShipmentOfferStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DriverShipmentOfferDTO Driver { get; set; }
        public VehicleShipmentOfferDTO Vehicle { get; set; }

    }

    public class DriverShipmentOfferDTO
    {
        public Guid DriverId { get; set; }
        public string DriverName { get; set; }
    }
    public class VehicleShipmentOfferDTO
    {
        public Guid VehicleId { get; set; }
        public string VehicleBrand { get; set; }
    }
}
