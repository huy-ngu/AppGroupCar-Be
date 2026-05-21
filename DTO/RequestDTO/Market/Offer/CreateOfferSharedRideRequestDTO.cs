using AppGroupCar.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Market.Offer
{
    public class CreateOfferSharedRidelRequestDTO
    {
        [Required]
        public Guid VehicleId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Range(1, 10000000, ErrorMessage = "Giá offer phải lớn hơn 0 và nhỏ hơn 10triệu")]
        public decimal BasePrice { get; set; } = 10000000;

        [Required]
        [MinLength(2)]
        public List<CreateOfferRoutePointDTO> OfferRoutePoints { get; set; } = new();
    }

    public class CreateOfferRoutePointDTO
    {
        [DefaultValue("89c8eb10-273a-4987-bc9c-08de832bc18c")]
        [Required]
        public Guid LocationId { get; set; }

        [Range((int)RouteStopType.Start, (int)RouteStopType.End, ErrorMessage = "StopType is invalid")]
        public RouteStopType StopType { get; set; }
    }
}
