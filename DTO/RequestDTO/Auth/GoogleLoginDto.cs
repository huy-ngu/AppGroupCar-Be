using System.ComponentModel;

namespace AppGroupCar.DTO.RequestDTO.Auth
{
    public class GoogleLoginDto
    {
        [DefaultValue("IDToken")]
        public string IdToken { get; set; } = null!;

    }
}
