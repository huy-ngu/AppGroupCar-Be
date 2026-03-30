using AppGroupCar.Entities.Market;
using System.ComponentModel;
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

        [Range(1, 10000000, ErrorMessage = "Giá offer phải lớn hơn 0 và nhỏ hơn 10triệu")]
        public decimal BasePrice { get; set; } = 10000000;

        [Required]
        public List<CreateOfferRoutePointDTO> OfferRoutePoints { get; set; }

    }

    public class CreateOfferRoutePointDTO
    {
        public int Sequence { get; set; }
        [DefaultValue("89c8eb10-273a-4987-bc9c-08de832bc18c")]
        public Guid LocationId { get; set; }
    }
}
