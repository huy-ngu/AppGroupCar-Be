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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            if (string.IsNullOrEmpty(email)) return BadRequest(new ApiResponse<string> { Message = "Email không được null" });

            var (shouldSendEmail, encodedToken) = await _userManager.ProcessResetTokenWithCacheAsync(email, _cache);
            if (shouldSendEmail)
            {
                string resetLink = $"https://huyapp.com/reset-password?email={email}&token={encodedToken}";
                string body = $"<p>Nhấn vào <a href='{resetLink}'>đây</a> để đổi mật khẩu. Link hết hạn sau 15 phút.</p>";

                _backgroundJobs.Enqueue<IEmailService>(x => x.SendEmailAsync(email, "Reset Password", body));
            }

            // LUÔN LUÔN TRẢ VỀ CÂU NÀY, DÙ CÓ THẤY USER HAY LÀ BỊ SPAM ĐI NỮA
            return Ok(new ApiResponse<string> { Message = "Vui lòng kiểm tra email của bạn." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            await _authService.ResetPassword(request);
            return Ok(new ApiResponse<string> { Message = "Đổi mật khẩu thành công" });
        }
    }
}
