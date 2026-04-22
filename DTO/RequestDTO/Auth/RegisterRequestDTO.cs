using System.ComponentModel;

namespace AppGroupCar.DTO.RequestDTO.Auth
{
    public class RegisterRequestDTO
    {
        public string FullName { get; set; }
        [DefaultValue("Email@gmail.com")]
        public string Email { get; set; }
        [DefaultValue("0123456789")]
        public string PhoneNumber { get; set; }
        //public string PhoneNumberConfirmed { get; set; }
        [DefaultValue("123456")]
        public string Password { get; set; }
        public IFormFile? Avatar { get; set; }

    }
}
