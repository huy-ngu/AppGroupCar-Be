using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.Entities.Bus
{
    [Table("Companies")]
    public class Company
    {
        [Key]
        public Guid CompanyId { get; set; }

        // Thông tin cơ bản
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string CompanyCode { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        // Thông tin liên hệ
        [Required]
        [MaxLength(15)]
        public string Phone { get; set; } = null!;

        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string ProvinceCode { get; set; } = null!;

        [MaxLength(20)]
        public string? DistrictCode { get; set; }

        // Thông tin pháp lý
        [Required]
        [MaxLength(50)]
        public string BusinessLicenseNo { get; set; } = null!;

        [MaxLength(50)]
        public string? TaxCode { get; set; }

        public DateTime? LicenseIssuedDate { get; set; }

        [MaxLength(200)]
        public string? LicenseIssuedBy { get; set; }

        // Thông tin vận hành
        [Required]
        public int CompanyType { get; set; }

        [Required]
        public int Status { get; set; }

        // Thông tin hệ thống
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedBy { get; set; }

        // Cờ hệ thống
        [Required]
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public ICollection<CompanyUser> CompanyUsers { get; set; } = new List<CompanyUser>();
    }
}
