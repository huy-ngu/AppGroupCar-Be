using System.ComponentModel;

namespace AppGroupCar.DTO.RequestDTO
{
    public class GoogleLoginDto
    {
        [DefaultValue("IDToken")]
        public string IdToken { get; set; } = null!;

    }
}
