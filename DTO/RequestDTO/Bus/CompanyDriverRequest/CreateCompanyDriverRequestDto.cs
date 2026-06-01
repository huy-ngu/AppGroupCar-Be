using Microsoft.AspNetCore.Http;

namespace AppGroupCar.DTO.RequestDTO.Bus.CompanyDriverRequest
{
    public class CreateCompanyDriverRequestDto
    {
        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Password { get; set; }

        public string LicenseNumber { get; set; } = null!;

        public string LicenseClass { get; set; } = null!;

        public int DriverVerificationStatus { get; set; }

        public IFormFile? LicenseDocumentImage { get; set; }
    }
}
