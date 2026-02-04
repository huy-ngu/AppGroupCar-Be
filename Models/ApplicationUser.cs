using Microsoft.AspNetCore.Identity;

namespace AppGroupCar.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

    }
}
