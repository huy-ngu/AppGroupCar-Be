using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.CompanyRequest
{
    public class ApproveCompanyRequestDto
    {
        [Required]
        public CompanyStatus Status { get; set; }

        public string? Description { get; set; }
    }
}
