using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.CompanyRequest
{
    public class CompanyLoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string BusinessLicenseNo { get; set; } = null!;
    }
}
