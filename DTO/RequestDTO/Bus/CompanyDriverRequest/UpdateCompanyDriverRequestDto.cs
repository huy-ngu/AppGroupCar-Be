namespace AppGroupCar.DTO.RequestDTO.Bus.CompanyDriverRequest
{
    public class UpdateCompanyDriverRequestDto
    {
        public string? LicenseNumber { get; set; }

        public string? LicenseClass { get; set; }

        public int? DriverVerificationStatus { get; set; }

        public string? LicenseDocumentUrl { get; set; }
    }
}
