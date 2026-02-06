using System.Security.Claims;
using AppGroupCar.Entities;

namespace AppGroupCar.Services.TokenServices
{
    public interface ITokenService
    {
        // returns (token string, expiry DateTime)
        public string CreateToken(ApplicationUser user, IList<string> roles);
        public RefreshToken CreateRefreshToken(string userId, string jwtId);
    }
}
