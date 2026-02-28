using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities;

namespace AppGroupCar.Repositories.RefreshTokensRepo
{
    public interface IRefreshTokenRepository
    {
        Task CreateTokenAsync(RefreshToken token);
        Task Revoke(RefreshToken token);
        Task Used(RefreshToken token);
        Task SaveAsync();
        Task<RefreshToken> GetByTokenAsync(string token);
        Task<bool> RevokeAllDevicesAsync(Guid userId);
    }
}
