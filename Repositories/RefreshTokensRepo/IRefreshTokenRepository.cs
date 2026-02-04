using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Models;

namespace AppGroupCar.Repositories.RefreshTokensRepo
{
    public interface IRefreshTokenRepository
    {
        Task CreateTokenAsync(RefreshToken token);
        Task Revoke(RefreshToken token);
        Task Used(RefreshToken token);
        Task SaveAsync();
        Task<RefreshToken> GetByTokenAsync(string token);
        Task<bool> RevokeAllDevicesAsync(string userId);
    }
}
