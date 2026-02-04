using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Models;

namespace AppGroupCar.Services.AuthServices
{
    public interface IAuthService
    {
        Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterRequestDTO request);
        Task<(bool Success, string AccessToken, string RefreshToken, string Error)> LoginAsync(LoginRequestDTO dto);
        Task<(bool Success, string AccessToken, string RefreshToken, string Error)> GoogleLoginAsync(GoogleLoginDto dto);
        Task<(string AccessToken, string RefreshToken)> Refresh(RefreshTokenRequest token);
        Task LogoutAllDevices(string userId);
        Task Logout(string token);
    }
}
