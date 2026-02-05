using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities
{
    [Table("VehicleSeatLayouts")]
    public class VehicleSeatLayout
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SeatLayoutId { get; set; }

        [Required]
        public int SeatCapacity { get; set; } // 16, 29, 45

        [Required]
        public string LayoutJson { get; set; } = null!; // sơ đồ ghế (JSON)

        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }

}
