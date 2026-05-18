namespace AppGroupCar.DTO.ResponseDTO.Bus.CompanyResponse
{
    public class CompanyLoginResultDto
    {
        public Guid CompanyId { get; set; }
        public CompanyStatus Status { get; set; }
        public string? Description { get; set; }
        public PendingCompanyUpdateDataDto? PendingUpdateData { get; set; }
    }

    public class PendingCompanyUpdateDataDto
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string ProvinceCode { get; set; } = null!;
        public string? DistrictCode { get; set; }
        public string BusinessLicenseNo { get; set; } = null!;
    }
}
