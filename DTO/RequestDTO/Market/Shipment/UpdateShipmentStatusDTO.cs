using AppGroupCar.Entities.Market;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.Shipment
{
    public class UpdateShipmentStatusDTO
    {
        [Required]
        public Guid ShipmentId { get; set; }

        [Required]
        public ShipmentStatus ShipmentStatus { get; set; }
    }
}
