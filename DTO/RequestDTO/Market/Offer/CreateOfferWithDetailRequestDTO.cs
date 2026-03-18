using AppGroupCar.Entities.Market;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.Offer
{
    public class CreateOfferWithDetailRequestDTO
    {
        public Guid VehicleId { get; set; }

        [Required]
        public ServiceType ServiceType { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Range(1, 100000000, ErrorMessage = "Giá offer phải lớn hơn 0 và nhỏ hơn 100triệu")]
        public decimal BasePrice { get; set; }

        [Required]
        public OfferStatus Status { get; set; }

    }
}
