using AppGroupCar.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.Entities.Bus
{
    [Table("Companies")]
    public class Company : BaseEntity
    {

        // Thông tin cơ bản
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string CompanyCode { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? LogoUrl { get; set; }

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
        public CompanyType CompanyType { get; set; }

        [Required]
        public CompanyStatus Status { get; set; }

        public decimal RatingCompanyAverage { get; set; }

        public int RatingCompanyCount { get; set; }

        //tạm bỏ
        // [Required]
        //public Guid CreatedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }
        // Thời điểm duyệt công ty
        //        public Guid? ApprovedBy { get; set; }// Người duyệt công ty

        // Navigation
        public ICollection<CompanyUser> CompanyUsers { get; set; } = new List<CompanyUser>();

        public ICollection<BusRoute> BusRoute { get; set; } = new List<BusRoute>();
    }
}

public enum CompanyType
{
    Bus = 1,
    CarRental = 2,
    RideSharing = 3
}

public enum CompanyStatus
{
    Pending = 0,
    Approved = 1,// công ty đã được duyệt, có thể tạo chuyến, không thể chỉnh sửa thông tin công ty, nếu muốn chỉnh sửa phải gửi yêu cầu chỉnh sửa và chờ duyệt lại
    Rejected = 2,// công ty bị từ chối duyệt, có thể chỉnh sửa lại thông tin và gửi lại yêu cầu duyệt
    Suspended = 3// công ty bị tạm ngưng hoạt động, không thể tạo chuyến, có thể chỉnh sửa lại thông tin và gửi lại yêu cầu duyệt
}