using AppGroupCar.DTO.RequestDTO.User;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Services.Auth.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<UserDto>> { Data = users });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(new ApiResponse<UserDto> { Data = user });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] CreateUserDTO dto)
        {
            await _userService.CreateAsync(dto);
            return Ok(new ApiResponse<object> { Data = null, Message = "User created" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Update(string id, [FromForm] UpdateUserDTO dto)
        {
            await _userService.UpdateAsync(id, dto);
            return Ok(new ApiResponse<object> { Data = null, Message = "User updated" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.DeleteAsync(id);
            return Ok(new ApiResponse<object> { Data = null, Message = "User deleted" });
        }

        [HttpPost("{id}/change-password")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordDTO dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest(new ApiResponse<object> { Message = "New password and confirm password do not match" });

            if (!Guid.TryParse(id, out Guid userId))
                return BadRequest(new ApiResponse<object> { Message = "Invalid user ID" });

            await _userService.ChangePassword(userId, dto.OldPassword, dto.NewPassword);
            return Ok(new ApiResponse<object> { Data = null, Message = "Password changed successfully" });
        }
    }
}
