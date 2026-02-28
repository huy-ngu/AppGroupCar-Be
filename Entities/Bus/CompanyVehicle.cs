using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Bus
{
    [Table("CompanyVehicles")]
    public class CompanyVehicle : BaseEntity
    {
        public Guid CompanyVehicleId { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid SeatLayoutId { get; set; }

        [Required]
        [MaxLength(20)]
        public string PlateNumber { get; set; } = null!;

        [Required]
        public int SeatCapacity { get; set; } // 29

        [Required]
        public int VehicleType { get; set; } // Coach

        [Required]
        public int Status { get; set; }

        // Navigation
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [ForeignKey(nameof(SeatLayoutId))]
        public VehicleSeatLayout VehicleSeatLayout { get; set; } = null!;
    }

}
