using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.RequestDTO
{
    public class CreateShipmentRequestDTO
    {
        [Required]
        public Guid PickupLocationId { get; set; }

        [Required]
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
        public bool IsFragile { get; set; } = false;

        [Required]
        public bool IsPerishable { get; set; } = false;

        [Required]
        public bool RequiresCooler { get; set; } = false;

        [Required]
        public bool AvoidSunLight { get; set; } = false;

        [MaxLength(500)]
        public string? HandlingNote { get; set; }
    }
}
