using System.Security.Claims;
using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Services.AuthServices;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO request)
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("Sai token");
            }
            await _authService.LogoutAllDevices(userId);
            return Ok(new ApiResponse<string> { });
        }
    }
}
