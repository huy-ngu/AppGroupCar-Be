using AppGroupCar.DTO.RequestDTO.Auth;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Auth.AuthServices;
using AppGroupCar.Services.Auth.EmailServices;
using CloudinaryDotNet.Actions;
using Google.Apis.Auth;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly UserManager<ApplicationUser> _userManager;
        private IMemoryCache _cache;

        public AuthController(
            IAuthService authService,
            IBackgroundJobClient backgroundJobs,
            UserManager<ApplicationUser> userManager,
            IMemoryCache cache
        )
        {
            _authService = authService;
            _backgroundJobs = backgroundJobs;
            _userManager = userManager;
            _cache = cache;
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
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "Email không được để trống"
                });
            }

            var (shouldSendEmail, otp) =
                await _userManager.ProcessResetOtpWithCacheAsync(
                    email,
                    _cache);

            if (shouldSendEmail)
            {
                string body = $@"
            <h3>Mã OTP đặt lại mật khẩu</h3>
            <p>Mã OTP của bạn là:</p>
            <h2>{otp}</h2>
            <p>Mã có hiệu lực trong 5 phút.</p>
        ";

                _backgroundJobs.Enqueue<IEmailService>(
                    x => x.SendEmailAsync(
                        email,
                        "Mã OTP đặt lại mật khẩu",
                        body));
            }

            return Ok(new ApiResponse<string>
            {
                Message = "Gửi Email thành công"
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequestDTO request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "OTP không hợp lệ hoặc đã hết hạn"
                });
            }

            string otpKey = $"ResetPassOtp_{request.Email}";

            if (!_cache.TryGetValue(otpKey, out string? cachedOtp))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "OTP đã hết hạn"
                });
            }

            if (cachedOtp != request.Otp)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "OTP không chính xác"
                });
            }

            // Tạo password hash bằng Identity
            user.PasswordHash = _userManager.PasswordHasher
                .HashPassword(user, request.NewPassword);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = string.Join(", ",
                        result.Errors.Select(x => x.Description))
                });
            }

            // OTP chỉ dùng 1 lần
            _cache.Remove(otpKey);

            return Ok(new ApiResponse<string>
            {
                Message = "Đổi mật khẩu thành công"
            });
        }
    }
}
