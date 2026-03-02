using System.ComponentModel;

namespace AppGroupCar.DTO.RequestDTO
{
    public class LoginRequestDTO
    {
        [DefaultValue("admin@example.com")]
        public string Email { get; set; }
        [DefaultValue("Admin@123")]
        public string Password { get; set; }
    }
}
