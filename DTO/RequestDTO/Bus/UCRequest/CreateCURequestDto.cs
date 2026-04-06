using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.UCRequest
{
    public class CreateCURequestDto
    {
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}
