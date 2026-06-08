using AppGroupCar.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
namespace AppGroupCar.Helpers;
public static class UserManagerExtensions
{
    // 1. Hàm static, IMemoryCache được truyền vào qua tham số
    // 2. Trả về Tuple: (bool ShouldSendEmail, string EncodedToken)
    public static async Task<(bool ShouldSendEmail, string Otp)> ProcessResetOtpWithCacheAsync(
      this UserManager<ApplicationUser> userManager,
      string email,
      IMemoryCache cache)
    {
        // Tìm user
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return (false, string.Empty);
        }

        // Chống spam gửi mail
        string cooldownKey = $"ResetPassCooldown_{email}";

        if (cache.TryGetValue(cooldownKey, out _))
        {
            return (false, string.Empty);
        }

        // Sinh OTP 6 số
        var otp = RandomNumberGenerator
            .GetInt32(100000, 1000000)
            .ToString();

        // Lưu OTP vào cache
        string otpKey = $"ResetPassOtp_{email}";

        cache.Set(
            otpKey,
            otp,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        // Cooldown gửi mail 60 giây
        cache.Set(
            cooldownKey,
            true,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        return (true, otp);
    }
}