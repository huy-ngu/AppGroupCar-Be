using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Bus
{
    [Table("CompanyDrivers")]
    public class CompanyDriver
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CompanyDriverId { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string LicenseNumber { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string LicenseClass { get; set; } = null!;

        [Required]
        public int VerificationStatus { get; set; } // Pending / Approved / Rejected

        public DateTime? VerifiedAt { get; set; }

        public Guid? VerifiedBy { get; set; }

        // Navigation
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
    }

}
