using AppGroupCar.Data;
using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities;
using AppGroupCar.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Repositories.Auth.RefreshTokensRepo
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateTokenAsync(RefreshToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token), "Bị hack rồi");
            }
            await _context.RefreshTokens.AddAsync(token);

            var result = await _context.SaveChangesAsync();

            if (result <= 0)
            {
                throw new AppException(500, "Lỗi lưu Refresh Token: Database không phản hồi thay đổi.");
            }

        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task Revoke(string token)
        {
            var rs = await _context.RefreshTokens.FindAsync(token);
            if (rs == null)
            {
                return;
            }
            rs.Invalidated = true;
            await _context.SaveChangesAsync();
        }

        public async Task Revoke(RefreshToken token)
        {
            token.Invalidated = true;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RevokeAllDevicesAsync(Guid userId)
        {
            var userActive = _context.RefreshTokens.Where(x => x.UserId == userId && !x.Invalidated).ToList();
            if (!userActive.Any())
            {
                return false;
            }
            foreach (var user in userActive)
            {
                user.Invalidated = true;
            }
            _context.RefreshTokens.UpdateRange(userActive);
            var saved = await _context.SaveChangesAsync();

            return saved > 0;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }


        public async Task Used(RefreshToken token)
        {
            token.Used = true;
            await _context.SaveChangesAsync();
        }
    }
}
