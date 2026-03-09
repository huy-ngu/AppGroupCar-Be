using AppGroupCar.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
namespace AppGroupCar.Helpers;
public static class UserManagerExtensions
{
    // 1. Hàm static, IMemoryCache được truyền vào qua tham số
    // 2. Trả về Tuple: (bool ShouldSendEmail, string EncodedToken)
    public static async Task<(bool ShouldSendEmail, string EncodedToken)> ProcessResetTokenWithCacheAsync(
        this UserManager<ApplicationUser> userManager,
        string email,
        IMemoryCache cache)
    {
        // BƯỚC 1: TÌM USER - VÀ XÓA NGAY CÁI THROW 404 ĐI!
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Trả về false, báo cho Controller biết là "Đừng gửi mail, tao không thấy user"
            return (false, string.Empty);
        }

        // BƯỚC 2: KIỂM TRA RAM (CACHE CHỐNG SPAM)
        string cacheKey = $"ResetPassWait_{email}";
        if (cache.TryGetValue(cacheKey, out _))
        {
            // Đã xin trong 15 phút qua rồi. Trả về false, KHÔNG LÀM GÌ CẢ.
            return (false, string.Empty);
        }

        // BƯỚC 3: TẠO VÀ ENCODE TOKEN LIỀN TAY
        var rawToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));

        // BƯỚC 4: LƯU CACHE 15 PHÚT
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
        cache.Set(cacheKey, true, cacheOptions);

        // BƯỚC 5: TRẢ VỀ THÀNH CÔNG VÀ KÈM TOKEN ĐÃ MÃ HÓA
        return (true, encodedToken);
    }
}