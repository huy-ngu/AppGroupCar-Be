namespace AppGroupCar.DTO.RequestDTO.Bus.CompanyDriverRequest
{
    public class CreateCompanyDriverRequestDto
    {
        public Guid UserId { get; set; }

        public string LicenseNumber { get; set; } = null!;

        public string LicenseClass { get; set; } = null!;

        public int DriverVerificationStatus { get; set; }

        public string? LicenseDocumentUrl { get; set; }
    }
}
