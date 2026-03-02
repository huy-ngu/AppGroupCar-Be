using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Market
{
    [Table("Vehicles")]
    public class Vehicle : BaseEntity
    {
        [Required]
        public Guid DriverId { get; set; }

        // Thông tin xe
        [Required]
        [MaxLength(20)]
        public string PlateNumber { get; set; } = null!; //biển số xe

        [Required]
        [MaxLength(50)]
        public string Brand { get; set; } = null!;

        [Required]
        public int SeatCapacity { get; set; }

        [Required]
        public VehicleType VehicleType { get; set; }

        [Required]
        public VehicleStatus Status { get; set; }

        [Required]
        [MaxLength(500)]
        public string? UrlImage { get; set; } //lưu giấy đăng kí xe có gắn với biển số

        [Required]
        [MaxLength(500)]
        public string? RegistrationDocumentUrl { get; set; } //ảnh đăng kiểm


        // Navigation
        [ForeignKey(nameof(DriverId))]
        public Driver Driver { get; set; } = null!;
    }

    public enum VehicleType
    {
        Car = 1,
        Truck = 2
    }

    public enum VehicleStatus
    {
        Active = 1,
        Inactive = 2,
        Maintenance = 3
    }
}
