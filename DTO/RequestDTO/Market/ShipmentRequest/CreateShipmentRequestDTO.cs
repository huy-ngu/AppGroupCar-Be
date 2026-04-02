using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.RequestDTO.Market.ShipmentRequest
{
    public class CreateShipmentRequestDTO
    {
        [Required]
        [DefaultValue("A63177D3-2C21-48C8-BC9B-08DE832BC18C")]
        public Guid PickupLocationId { get; set; }

        [Required]
        [DefaultValue("89C8EB10-273A-4987-BC9C-08DE832BC18C")]
        public Guid DropoffLocationId { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [DefaultValue(5)]
        public decimal Weight { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [DefaultValue(5)]
        public decimal Volume { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // Flags
        [Required]
        [DefaultValue(false)]
        public bool IsFragile { get; set; } = false;

        [Required]
        [DefaultValue(false)]
        public bool IsPerishable { get; set; } = false;

        [Required]
        [DefaultValue(false)]
        public bool RequiresCooler { get; set; } = false;

        [Required]
        [DefaultValue(false)]
        public bool AvoidSunLight { get; set; } = false;

        [MaxLength(500)]
        [DefaultValue("")]
        public string? HandlingNote { get; set; }
        public IFormFile? ImageUrl { get; set; }
    }
}
