using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.RequestDTO.Market.Shipment
{
    public class CreateShipmentOfferFromOfferRequestDTO
    {
        [Required]
        public Guid OfferId { get; set; }

        [Required]
        public Guid RequestId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ProposedPrice { get; set; }
    }
}
