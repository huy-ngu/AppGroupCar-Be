using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Bus
{
    [Table("CompanyDrivers")]
    public class CompanyDriver : BaseEntity
    {

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
        public int DriverVerificationStatus { get; set; }

        [MaxLength(500)]
        public string? LicenseDocumentUrl { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public Guid? VerifiedBy { get; set; }

        // Navigation
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;
    }

}
