using AppGroupCar.DTO.RequestDTO.Auth;
using AppGroupCar.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppGroupCar.Repositories.Auth.UserRepo
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles);
        Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo loginInfo);
        Task<ApplicationUser> FindByIdAsync(Guid id);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);

    }
}
