using AppGroupCar.Entities.Market;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.Offer
{
    public class CreateOfferRequestDTO
    {

        [Required]
        public Guid VehicleId { get; set; }

        [Required]
        public ServiceType ServiceType { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        public decimal BasePrice { get; set; }

        [Required]
        public OfferStatus Status { get; set; }
    }
}
