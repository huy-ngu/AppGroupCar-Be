using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO.Auth
{
    public class ResetPasswordRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự!")]
        public string NewPassword { get; set; }
    }
}
