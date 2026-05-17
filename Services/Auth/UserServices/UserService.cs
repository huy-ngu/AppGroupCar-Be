using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.User;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Identity;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Services.Mytoys.Photo;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PagedResult<UserDto>> GetAllAsync(UserParameters param)
        {
            // 1. Khởi tạo Query (Tuyệt đối CHƯA gọi ToList ở đây)
            var query = _userManager.Users.AsQueryable();

            // 2. Lọc theo Search (FullName, PhoneNumber, Email)
            if (!string.IsNullOrWhiteSpace(param.Search))
            {
                var searchTerm = param.Search.Trim().ToLower();
                query = query.Where(u =>
                    (u.FullName != null && u.FullName.ToLower().Contains(searchTerm)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(searchTerm)) ||
                    (u.Email != null && u.Email.ToLower().Contains(searchTerm))
                );
            }

            // 3. Lọc theo IsActive (Dựa vào trường DeletedAt)
            if (param.isActive.HasValue)
            {
                if (param.isActive.Value)
                {
                    query = query.Where(u => u.DeleteAt == null); // Đang hoạt động
                }
                else
                {
                    query = query.Where(u => u.DeleteAt != null); // Đã bị xóa (Soft Delete)
                }
            }

            // 4. Lọc theo CreatedAt (Xử lý thông minh)
            if (!string.IsNullOrWhiteSpace(param.CreatedAt))
            {
                var dateStr = param.CreatedAt.Trim();
                // Cắt chuỗi theo dấu "-" (Ví dụ: "2026-04" hoặc "2026-04-15")
                var dateParts = dateStr.Split('-');

                if (dateParts.Length == 2 && int.TryParse(dateParts[0], out int year) && int.TryParse(dateParts[1], out int month))
                {
                    // Tình huống Frontend chỉ gửi Tháng và Năm ("yyyy-MM")
                    query = query.Where(u => u.CreatedAt.Year == year && u.CreatedAt.Month == month);
                }
                else if (DateTime.TryParse(dateStr, out DateTime exactDate))
                {
                    // Tình huống Frontend gửi đầy đủ Ngày Tháng Năm ("yyyy-MM-dd")
                    query = query.Where(u => u.CreatedAt.Date == exactDate.Date);
                }
            }

            // 5. Tính tổng số bản ghi thỏa mãn điều kiện (Dùng cho UI phân trang)
            var totalCount = await query.CountAsync();

            // 6. Thực thi Phân trang (Lúc này mới gọi xuống Database để lấy đúng số lượng cần thiết)
            var pagedUsers = await query
                .Skip((param.PageNumber - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToListAsync();

            // 7. Map sang DTO và xử lý Role
            var resultList = new List<UserDto>();
            foreach (var u in pagedUsers)
            {
                var userDto = _mapper.Map<UserDto>(u);

                // Vẫn phải dùng vòng lặp, nhưng lúc này nó chỉ chạy đúng 10 lần (PageSize) thay vì cả ngàn lần.
                userDto.Roles = await _userManager.GetRolesAsync(u);

                resultList.Add(userDto);
            }

            // 8. Trả về đúng format PagedResult
            return new PagedResult<UserDto>
            {
                Items = resultList,         // Danh sách DTO đã map
                TotalCount = totalCount,    // Tổng số lượng
                PageNumber = param.PageNumber,
                PageSize = param.PageSize
            };
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

        public async Task<UserDto> CreateAsync(CreateUserDTO dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);

            if (existing != null)
                throw new AppException(400, "Email already exists");

            string? avatarUrl = null;

            var user = _mapper.Map<ApplicationUser>(dto);

            if (dto.AvatarUrl != null && dto.AvatarUrl.Length > 0)
            {
                var uploadResult = await _photoService.UploadPhotoAsync(
                    dto.AvatarUrl,
                    "user-avatars"
                );

                if (uploadResult == null)
                    throw new AppException(400, "Avatar upload failed");

                if (uploadResult.Error != null)
                    throw new AppException(
                        400,
                        "Cloudinary upload failed: " + uploadResult.Error.Message
                    );

                avatarUrl = uploadResult.SecureUrl.AbsoluteUri;

                user.AvatarUrl = avatarUrl;
            }

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                throw new AppException(
                    400,
                    "Create user failed",
                    new
                    {
                        errors = result.Errors.Select(e => e.Description)
                    }
                );
            }

            // thêm role nếu chưa có
            if (dto.Roles != null && dto.Roles.Any())
            {
                foreach (var role in dto.Roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                        throw new AppException(404, "Role not found: " + role);
                }

                var addRoleResult = await _userManager.AddToRolesAsync(user, dto.Roles);

                if (!addRoleResult.Succeeded)
                {
                    throw new AppException(
                        400,
                        "Add role failed",
                        new
                        {
                            errors = addRoleResult.Errors.Select(e => e.Description)
                        }
                    );
                }
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userResponse = _mapper.Map<UserDto>(user);

            userResponse.Roles = roles;

            return userResponse;
        }

        public async Task UpdateAsync(string id, UpdateUserDTO dto)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                throw new AppException(404, "User not found");

            user.FullName = dto.FullName ?? user.FullName;

            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

            if (dto.ImgFile != null && dto.ImgFile.Length > 0)
            {
                var uploadResult = await _photoService.UploadPhotoAsync(
                    dto.ImgFile,
                    "user-avatars"
                );

                if (uploadResult == null)
                    throw new AppException(400, "Avatar upload failed");

                if (uploadResult.Error != null)
                {
                    throw new AppException(
                        400,
                        "Cloudinary upload failed: " + uploadResult.Error.Message
                    );
                }

                user.AvatarUrl = uploadResult.SecureUrl.AbsoluteUri;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new AppException(
                    400,
                    "Update failed",
                    new
                    {
                        errors = result.Errors.Select(e => e.Description)
                    }
                );
            }
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new AppException(404, "User not found");
            user.DeleteAt = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new AppException(400, "Delete failed", new { errors = result.Errors.Select(e => e.Description) });
        }

        public async Task RestoreAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new AppException(404, "Không tìm thấy người dùng");
            user.DeleteAt = null;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) throw new AppException(400, "Restore failed", new { errors = result.Errors.Select(e => e.Description) });
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
