using AppGroupCar.Entities.Market;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.Offer
{
    public class CreateOfferShipmentRequestDTO
    {
        public Guid VehicleId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; } = DateTime.Now.AddMinutes(30);

        [Range(1, 10080, ErrorMessage = "EstimatedDurationMinutes phai lon hon 0 va nho hon 10080")]
        public int EstimatedDurationMinutes { get; set; }

        [Range(1, 10000000, ErrorMessage = "Giá offer phải lớn hơn 0 và nhỏ hơn 10triệu")]
        public decimal BasePrice { get; set; } = 10000000;

        [Required]
        [MinLength(2)]
        public List<CreateOfferRoutePointDTO> OfferRoutePoints { get; set; } = new();

        [Required]
        public CreateCargoRequestDTO Cargo { get; set; }
    }
}
