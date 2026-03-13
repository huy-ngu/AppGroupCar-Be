using System.ComponentModel;

namespace AppGroupCar.DTO.RequestDTO.User
{
    public class UpdateUserDTO
    {
        [DefaultValue("")]
        public string? FullName { get; set; }
        [DefaultValue("")]
        public string? PhoneNumber { get; set; }
        public IFormFile? ImgFile { get; set; }
        //public IList<string> Roles { get; set; }
    }
}
