using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.RequestDTO.Market.ShipmentRequest
{
    public class CreateShipmentRequestDTO
    {
        [Required]
        [DefaultValue("2576628b-2325-45a4-33ad-08de76eb22d1")]
        public Guid PickupLocationId { get; set; }

        [Required]
        [DefaultValue("a4054762-067d-4627-35bc-08de76ebe4bb")]
        public Guid DropoffLocationId { get; set; }

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
