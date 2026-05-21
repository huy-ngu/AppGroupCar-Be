using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.RequestDTO.Market.Shipment
{
    public class CreateShipmentOfferRequestDTO
    {
        [Required]
        public Guid RequestId { get; set; }
        [Required]
        public Guid OfferId { get; set; }
        [Required]
        public Guid VehicleId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, 10000000, ErrorMessage = "Giá offer phải lớn hơn 0 và nhỏ hơn 10triệu")]

        public decimal ProposedPrice { get; set; }

    }
}
