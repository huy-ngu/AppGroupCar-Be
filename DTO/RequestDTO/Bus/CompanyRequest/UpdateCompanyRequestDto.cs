using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.CompanyRequest
{
    public class UpdateCompanyRequestDto
    {
        public string? CompanyName { get; set; } = null!;

        //public string? Description { get; set; }

        public IFormFile? Logo { get; set; }

        [Phone]
        public string? Phone { get; set; } = null!;

        //[EmailAddress]
        //public string? Email { get; set; }

        public string? Address { get; set; } = null!;

        public string? ProvinceCode { get; set; } = null!;

        public string? DistrictCode { get; set; }
    }
}
