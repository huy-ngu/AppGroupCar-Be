using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.User;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Services.Auth.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAll([FromQuery] UserParameters param)
        {
            var users = await _userService.GetAllAsync(param);
            return Ok(new ApiResponse<PagedResult<UserDto>> { Data = users });
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
            var user = await _userService.CreateAsync(dto);
            return Ok(new ApiResponse<UserDto> { Data = user, Message = "User created" });
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

        [HttpPost("{id}/restore")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Restore(string id)
        {
            await _userService.RestoreAsync(id);
            return Ok(new ApiResponse<object> { Data = null, Message = "User restored" });
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
