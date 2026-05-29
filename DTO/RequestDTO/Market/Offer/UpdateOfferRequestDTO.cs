using AppGroupCar.Entities.Market;
using System;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.Offer
{
    public class UpdateOfferRequestDTO
    {
        public Guid Id { get; set; }
        public Guid? VehicleId { get; set; }
        public ServiceType? ServiceType { get; set; }
        public DateTime? DepartureTime { get; set; }

        [Range(1, 10080, ErrorMessage = "EstimatedDurationMinutes phai lon hon 0 va nho hon 10080")]
        public int? EstimatedDurationMinutes { get; set; }

        public decimal? BasePrice { get; set; }
        public OfferStatus? Status { get; set; }
    }
}
