using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Market
{
    [Table("Drivers")]
    public class Driver
    {
        [Key]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string IdentityNumber { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string LicenseNumber { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string LicenseClass { get; set; } = null!;

        [Required]
        public DriverVerificationStatus VerificationStatus { get; set; }

        // Navigation
        public User User { get; set; } = null!;

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }

    public enum DriverVerificationStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }

}
