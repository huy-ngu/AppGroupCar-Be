using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography; // Cần cái này để tạo Refresh Token chuẩn
using System.Text;
using AppGroupCar.Entities;
using AppGroupCar.Services.Auth.TokenServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(ApplicationUser user, IList<string> roles)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new Claim("fullname", user.FullName ?? string.Empty)
    };

        if (roles != null && roles.Any())
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim("Roles", role));
            }
        }

        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        // TRICK LỎ: Xóa nbf và iat bằng cách set flag này về false
        tokenHandler.SetDefaultTimesOnTokenCreation = false;

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public RefreshToken CreateRefreshToken(Guid userId, string jwtId)
    {
        var refreshToken = new RefreshToken
        {
            Token = GetRandomString(32), // Tạo chuỗi ngẫu nhiên 32 byte (base64)
            UserId = userId,
            JwtId = jwtId, // Link với AccessToken vừa tạo
            ExpiryDate = DateTime.UtcNow.AddDays(7), // Refresh token sống 7 ngày
            Used = false,
            Invalidated = false
        };

        return refreshToken;
    }

    private string GetRandomString(int length)
    {
        var randomBytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }


}