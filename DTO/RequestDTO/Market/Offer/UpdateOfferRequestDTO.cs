using AppGroupCar.Entities.Market;
using System;

namespace AppGroupCar.DTO.RequestDTO.Market.Offer
{
    public class UpdateOfferRequestDTO
    {
        public Guid Id { get; set; }
        public Guid? VehicleId { get; set; }
        public ServiceType? ServiceType { get; set; }
        public DateTime? DepartureTime { get; set; }
        public decimal? BasePrice { get; set; }
        public OfferStatus? Status { get; set; }
    }
}
