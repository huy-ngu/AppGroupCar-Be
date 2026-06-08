using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Auth
{
    public class ResetPasswordRequestDTO
    {
        public string Email { get; set; } = string.Empty;

        public string Otp { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }
}
