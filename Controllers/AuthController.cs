using AppGroupCar.DTO.RequestDTO.Auth;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Auth.AuthServices;
using AppGroupCar.Services.Auth.EmailServices;
using Google.Apis.Auth;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IBackgroundJobClient _backgroundJobs;
        public AuthController(IAuthService authService, IBackgroundJobClient backgroundJobs)
        {
            _authService = authService;
            _backgroundJobs = backgroundJobs;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequestDTO request)
        {
            await _authService.RegisterAsync(request); // will throw on error
            return Ok(new ApiResponse<object> { Data = null, Message = "Register success" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO dto)
        {
            var (success, token, refreshToken, error) = await _authService.LoginAsync(dto);

            var data = new AuthResponseDTO { AccessToken = token, RefreshToken = refreshToken };
            return Ok(new ApiResponse<AuthResponseDTO> { Data = data, Message = "Login success" });
        }

        [HttpPost("google/callback")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginDto dto)
        {
            var (success, token, refreshToken, error) = await _authService.GoogleLoginAsync(dto);
            var data = new AuthResponseDTO { AccessToken = token, RefreshToken = refreshToken };
            return Ok(new ApiResponse<AuthResponseDTO> { Data = data, Message = "Login success" });
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest token)
        {
            var (accessToken, refreshToken) = await _authService.Refresh(token);
            var data = new AuthResponseDTO { AccessToken = accessToken, RefreshToken = refreshToken };
            return Ok(new ApiResponse<AuthResponseDTO> { Data = data });

        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string RefreshToken)
        {
            await _authService.Logout(RefreshToken);
            return Ok(new ApiResponse<string> { });
        }
        [Authorize]
        [HttpPost("logout_AllDevices")]
        public async Task<IActionResult> LogoutAllDevices()
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            await _authService.LogoutAllDevices(userId);
            return Ok(new ApiResponse<string> { });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest(new ApiResponse<string> { Message = "Email không được null" });

            string token = "reset_password_token";

            string body = $"<h1>Khôi phục mật khẩu</h1><p>Click <a href='https://app.com/reset?token={token}'>vào đây</a>.</p>";

            // Enqueue đưa job vào hàng đợi và chạy ngầm.
            // Hangfire sẽ tự động resolve (tìm) IEmailService và gọi hàm SendEmailAsync.
            _backgroundJobs.Enqueue<IEmailService>(x => x.SendEmailAsync(email, "Reset Password", body));

            // Phản hồi về FE ngay lập tức, tốn chưa tới 10ms!
            return Ok(new { Message = "Yêu cầu đã được tiếp nhận. Vui lòng kiểm tra email." });
        }
    }
}
