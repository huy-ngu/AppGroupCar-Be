namespace AppGroupCar.DTO.ResponseDTO.Bus.CompanyResponse
{
    public class CompanyDetailResponseDto
    {
        public Guid Id { get; set; }

        // Cơ bản
        public string CompanyName { get; set; } = null!;
        public string CompanyCode { get; set; } = null!;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }

        // Liên hệ
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public string Address { get; set; } = null!;
        public string ProvinceCode { get; set; } = null!;
        public string? DistrictCode { get; set; }

        // Pháp lý
        public string BusinessLicenseNo { get; set; } = null!;
        public string? TaxCode { get; set; }
        public DateTime? LicenseIssuedDate { get; set; }
        public string? LicenseIssuedBy { get; set; }

        // Vận hành
        public int CompanyType { get; set; }
        public CompanyStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }
    }
}
