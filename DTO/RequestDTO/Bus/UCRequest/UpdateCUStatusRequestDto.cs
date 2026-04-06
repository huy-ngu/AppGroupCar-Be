using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Bus.UCRequest
{
    public class UpdateCUStatusRequestDto
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
