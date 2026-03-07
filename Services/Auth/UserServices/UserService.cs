using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppGroupCar.DTO.RequestDTO.User;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Identity;
using AppGroupCar.Exceptions;
using AppGroupCar.Services.Mytoys.Photo;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AppGroupCar.Services.Auth.UserServices
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IPhotoService photoService, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _photoService = photoService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserDto>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                UserDto user = _mapper.Map<UserDto>(u);
                user.Roles = roles;
                result.Add(user);
            }

            return result;
        }

        public async Task<UserDto> GetByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new AppException(404, "User not found");
            var roleName = await _userManager.GetRolesAsync(user);
            //return new UserDto
            //{
            //    Id = user.Id,
            //    UserName = user.UserName,
            //    Email = user.Email,
            //    FullName = user.FullName,
            //    PhoneNumber = user.PhoneNumber,
            //    Roles = roles
            //};
            var userResponse = _mapper.Map<UserDto>(user);
            userResponse.Roles = roleName;
            return userResponse;
        }

        public async Task CreateAsync(CreateUserDTO dto)
        {
            //var existing = await _userManager.FindByEmailAsync(dto.Email);
            //if (existing != null) throw new AppException(400, "Email already exists");
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                throw new AppException(400, "Email already exists");

            string? avatarUrl = null;
            var user = _mapper.Map<ApplicationUser>(dto);

            if (dto.AvatarUrl != null && dto.AvatarUrl.Length > 0)
            {
                var uploadResult = await _photoService.AddPhotoAsync(dto.AvatarUrl);

                if (uploadResult.Error != null)
                    throw new AppException(400, "Cloudinary upload failed: " + uploadResult.Error.Message);

                avatarUrl = uploadResult.SecureUrl.AbsoluteUri;
                user.AvatarUrl = avatarUrl;
            }

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new AppException(400, "Create user failed",
                    new { errors = result.Errors.Select(e => e.Description) });

            // thêm role nếu chưa có
            if (dto.Roles != null && dto.Roles.Any())
            {
                foreach (var role in dto.Roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                        throw new AppException(404, "Role not found: " + role);
                }

                await _userManager.AddToRolesAsync(user, dto.Roles);
            }
        }

        public async Task UpdateAsync(string id, UpdateUserDTO dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new AppException(404, "User not found");

            string? avatarUrl = null;

            user.FullName = dto.FullName ?? user.FullName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

            if (dto.ImgFile != null && dto.ImgFile.Length > 0)
            {
                var uploadResult = await _photoService.AddPhotoAsync(dto.ImgFile);

                if (uploadResult.Error != null)
                    throw new AppException(400, "Cloudinary upload failed: " + uploadResult.Error.Message);

                avatarUrl = uploadResult.SecureUrl.AbsoluteUri;
                user.AvatarUrl = avatarUrl;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new AppException(400, "Update failed", new { errors = result.Errors.Select(e => e.Description) });

        }

        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new AppException(404, "User not found");
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) throw new AppException(400, "Delete failed", new { errors = result.Errors.Select(e => e.Description) });
        }

        public async Task ChangePassword(Guid id, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new AppException(404, "User not found");

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (!result.Succeeded)
                throw new AppException(400, "Change password failed", 
                    new { errors = result.Errors.Select(e => e.Description) });
        }
    }
}
