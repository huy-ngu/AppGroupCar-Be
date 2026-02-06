using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities;
using AppGroupCar.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace AppGroupCar.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserDto>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                result.Add(new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    Roles = roles
                });
            }

            return result;
        }

        public async Task<UserDto> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new AppException(404, "User not found");
            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            };
        }

        public async Task CreateAsync(CreateUserDTO dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null) throw new AppException(400, "Email already exists");
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) throw new AppException(400, "Create user failed", new { errors = result.Errors.Select(e => e.Description) });

            if (dto.Roles != null && dto.Roles.Any())
            {
                foreach (var role in dto.Roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                        throw new AppException(404, "Role not found");
                    //await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRolesAsync(user, dto.Roles);
            }
        }

        public async Task UpdateAsync(string id, UpdateUserDTO dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new AppException(404, "User not found");
            user.FullName = dto.FullName ?? user.FullName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new AppException(400, "Update failed", new { errors = result.Errors.Select(e => e.Description) });

            //roles sync
            //if (dto.Roles != null)
            //{
            //    var current = await _userManager.GetRolesAsync(user);
            //    var toAdd = dto.Roles.Except(current).ToList();
            //    var toRemove = current.Except(dto.Roles).ToList();
            //    foreach (var role in toAdd)
            //    {
            //        if (!await _roleManager.RoleExistsAsync(role)) await _roleManager.CreateAsync(new IdentityRole(role));
            //    }
            //    if (toRemove.Any()) await _userManager.RemoveFromRolesAsync(user, toRemove);
            //    if (toAdd.Any()) await _userManager.AddToRolesAsync(user, toAdd);
            //}
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new AppException(404, "User not found");
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) throw new AppException(400, "Delete failed", new { errors = result.Errors.Select(e => e.Description) });
        }
    }
}
