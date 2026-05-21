using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Market
{
    [Table("Drivers")]
    public class Driver : BaseEntity
    {
        [Key]
        [Column("UserId")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string IdentityNumber { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string LicenseNumber { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string LicenseClass { get; set; } = null!;

        [Required]
        public DriverVerificationStatus VerificationStatus { get; set; }

        public string? LicenseDocumentUrl { get; set; }

        public decimal DriverRatingAverage { get; set; } = 5.00m;

        public int DriverRatingCount { get; set; } = 0;

        // Navigation
        [ForeignKey(nameof(Id))]
        public ApplicationUser User { get; set; } = null!;

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }

    public enum DriverVerificationStatus
    {
        Pending = 1,
        Active = 2,
        Inactive = 3,
        Rejected = 4
    }

}
