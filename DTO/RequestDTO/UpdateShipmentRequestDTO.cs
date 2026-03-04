using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.RequestDTO
{
    public class UpdateShipmentRequestDTO
    {
        //[Required]
        //[DefaultValue("552bfc5a-77e7-49d4-af3c-08de79a495a7")]
        //public Guid Id { get; set; }
        [DefaultValue("2576628b-2325-45a4-33ad-08de76eb22d1")]
        public Guid? PickupLocationId { get; set; }

        [DefaultValue("a4054762-067d-4627-35bc-08de76ebe4bb")]
        public Guid? DropoffLocationId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [DefaultValue(5)]
        public decimal Weight { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [DefaultValue(5)]
        public decimal Volume { get; set; }

        [MaxLength(500)]
        [DefaultValue("")]
        public string? Description { get; set; }

        // Flags
        [DefaultValue(false)]
        public bool IsFragile { get; set; } = false;

        [DefaultValue(false)]
        public bool IsPerishable { get; set; } = false;

        [DefaultValue(false)]
        public bool RequiresCooler { get; set; } = false;

        [DefaultValue(false)]
        public bool AvoidSunLight { get; set; } = false;

        [MaxLength(500)]
        [DefaultValue("")]
        public string? HandlingNote { get; set; }
    }
}
