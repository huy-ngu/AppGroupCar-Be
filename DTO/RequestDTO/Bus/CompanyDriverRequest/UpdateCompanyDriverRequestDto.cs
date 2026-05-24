using Microsoft.AspNetCore.Http;

namespace AppGroupCar.DTO.RequestDTO.Bus.CompanyDriverRequest
{
    public class UpdateCompanyDriverRequestDto
    {
        public string? LicenseNumber { get; set; }

        public string? LicenseClass { get; set; }

        public int? DriverVerificationStatus { get; set; }

        public IFormFile? LicenseDocumentImage { get; set; }
    }
}
