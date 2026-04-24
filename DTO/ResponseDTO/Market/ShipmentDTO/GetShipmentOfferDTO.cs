using AppGroupCar.Entities.Market;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO
{
    public class GetShipmentOfferDTO
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
        public decimal ProposedPrice { get; set; }
        public ShipmentOfferStatus Status { get; set; }
        public Guid? OfferId { get; set; }

    }
}
