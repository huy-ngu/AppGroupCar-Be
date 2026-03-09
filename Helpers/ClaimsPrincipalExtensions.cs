using AppGroupCar.Entities;
using AppGroupCar.Exceptions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppGroupCar.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Lấy UserId từ Token một cách an toàn. Trả về null nếu Token rác.
        /// </summary>
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdString, out Guid userId))
            {
                return userId;
            }

            return null;
        }

    }
}
