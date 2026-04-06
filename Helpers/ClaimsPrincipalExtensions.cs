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

        /// <summary>
        /// Lấy CompanyId từ claim "companyId". Trả về null nếu không có hoặc sai định dạng.
        /// </summary>
        public static Guid? GetCompanyId(this ClaimsPrincipal user)
        {
            var companyIdString = user.FindFirstValue("companyId");

            if (Guid.TryParse(companyIdString, out Guid companyId))
            {
                return companyId;
            }

            return null;
        }

        /// <summary>
        /// Lấy FullName từ claim "fullname". Trả về null nếu không có hoặc rỗng.
        /// </summary>
        public static string? GetFullName(this ClaimsPrincipal user)
        {
            var fullName = user.FindFirstValue("fullname");

            return string.IsNullOrWhiteSpace(fullName) ? null : fullName;
        }

    }
}
