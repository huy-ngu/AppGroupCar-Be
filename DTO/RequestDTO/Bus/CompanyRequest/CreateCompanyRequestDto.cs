using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.CompanyRequest
{
    public class CreateCompanyRequestDto
    {
        [Required]
        public string CompanyName { get; set; } = null!;

        [Required]
        public string CompanyCode { get; set; } = null!;

        //public string? Description { get; set; }

        public IFormFile? Logo { get; set; }

        // Thông tin liên hệ
        [Required]
        [Phone]
        public string Phone { get; set; } = null!;

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string ProvinceCode { get; set; } = null!;

        public string? DistrictCode { get; set; }

        // Pháp lý
        [Required]
        public string BusinessLicenseNo { get; set; } = null!;

        public string? TaxCode { get; set; }

        public DateTime? LicenseIssuedDate { get; set; }

        public string? LicenseIssuedBy { get; set; }

        // Loại công ty
        [Required]
        [Range(1, 5, ErrorMessage = "Giá trị phải nằm trong khoảng từ 1 đến 5.")]
        public int CompanyType { get; set; }
    }
}
