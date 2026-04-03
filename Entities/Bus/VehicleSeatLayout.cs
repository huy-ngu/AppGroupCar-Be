using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Bus
{
    [Table("VehicleSeatLayouts")]
    public class VehicleSeatLayout : BaseEntity
    {

        [Required]
        public string SeatLayoutName{ get; set; }
        [Required]
        public int SeatCapacity { get; set; } // 16, 29, 45

        [Required]
        public string LayoutJson { get; set; } = null!; // sơ đồ ghế (JSON)

        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

    }

}
