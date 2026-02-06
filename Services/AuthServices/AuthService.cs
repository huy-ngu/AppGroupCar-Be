using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Entities;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.RefreshTokensRepo;
using AppGroupCar.Repositories.UserRepo;
using AppGroupCar.Services.TokenServices;
using Azure.Core;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace AppGroupCar.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public AuthService(IUserRepository userRepository, IConfiguration configuration, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterRequestDTO request)
        {
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _userRepository.CreateUserAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                throw new AppException(400, "Register failed", new { errors });
            }

            // assign default role 'User'
            var addRoleResult = await _userRepository.AddToRolesAsync(user, new[] { "User" });
            if (!addRoleResult.Succeeded)
            {
                var errors = addRoleResult.Errors.Select(e => e.Description);
                throw new AppException(500, "Assign role failed", new { errors });
            }

            return (true, Array.Empty<string>());
        }

        public async Task<(bool Success, string AccessToken, string RefreshToken, string Error)> LoginAsync(LoginRequestDTO dto)
        {
            var user = await _userRepository.FindByEmailAsync(dto.Email);

            if (user == null)
                throw new AppException(401, "Invalid email");

            var valid = await _userRepository.CheckPasswordAsync(user, dto.Password);
            if (!valid)
                throw new AppException(401, "Invalid password");

            var roles = await _userRepository.GetRolesAsync(user);
            var AccessToken = _tokenService.CreateToken(user, roles);

            var jti = getJwtId(AccessToken);

            var RefreshToken = _tokenService.CreateRefreshToken(user.Id, jti);

            await _refreshTokenRepository.CreateTokenAsync(RefreshToken);


            return (true, AccessToken, RefreshToken.Token, null);
        }

        public async Task<(bool Success, string AccessToken, string RefreshToken, string Error)> GoogleLoginAsync(GoogleLoginDto dto)
        {
            // validate token
            var clientId = _configuration["Authentication:Google:ClientId"] ?? _configuration["Google:ClientId"];
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings();
                if (!string.IsNullOrEmpty(clientId)) settings.Audience = new[] { clientId };
                payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);
            }
            catch (Exception ex)
            {
                throw new AppException(400, "Invalid Google token", new { detail = ex.Message });
            }

            var email = payload.Email;
            var name = payload.Name;
            var googleUserId = payload.Subject;

            // try find by login
            var user = await _userRepository.FindByLoginAsync("Google", googleUserId);

            if (user == null)
            {
                // try find by email
                user = await _userRepository.FindByEmailAsync(email);
                if (user == null)
                {
                    // create user without password
                    var newUser = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = name,
                        EmailConfirmed = true
                    };
                    var createResult = await _userRepository.CreateUserAsync(newUser);
                    if (!createResult.Succeeded)
                    {
                        var errors = createResult.Errors.Select(e => e.Description);
                        throw new AppException(400, "Create user failed", new { errors });
                    }

                    // assign default role
                    var addRoleResult = await _userRepository.AddToRolesAsync(newUser, new[] { "User" });
                    if (!addRoleResult.Succeeded)
                    {
                        var errors = addRoleResult.Errors.Select(e => e.Description);
                        throw new AppException(500, "Assign role failed", new { errors });
                    }

                    // add external login
                    var loginInfo = new UserLoginInfo("Google", googleUserId, "Google");
                    var addLoginResult = await _userRepository.AddLoginAsync(newUser, loginInfo);
                    if (!addLoginResult.Succeeded)
                    {
                        var errors = addLoginResult.Errors.Select(e => e.Description);
                        throw new AppException(500, "Add external login failed", new { errors });
                    }

                    user = newUser;
                }
                else
                {
                    // user exists by email but not linked: add login
                    var loginInfo = new UserLoginInfo("Google", googleUserId, "Google");
                    var addLoginResult = await _userRepository.AddLoginAsync(user, loginInfo);
                    if (!addLoginResult.Succeeded)
                    {
                        var errors = addLoginResult.Errors.Select(e => e.Description);
                        throw new AppException(500, "Add external login failed", new { errors });
                    }
                }
            }

            // generate token
            var roles = await _userRepository.GetRolesAsync(user);
            var AccessToken = _tokenService.CreateToken(user, roles);

            var jti = getJwtId(AccessToken);

            var RefreshToken = _tokenService.CreateRefreshToken(user.Id, jti);

            await _refreshTokenRepository.CreateTokenAsync(RefreshToken);

            return (true, AccessToken, RefreshToken.Token, null);
        }

        public async Task<(string AccessToken, string RefreshToken)> Refresh(RefreshTokenRequest token)
        {
            if (token.AccessToken == null || token.RefreshToken == null)
            {
                throw new AppException(400, "Token không được rỗng");
            }
            var tokenDB = await _refreshTokenRepository.GetByTokenAsync(token.RefreshToken);
            if (tokenDB == null)
            {
                throw new AppException(401, "Không tìm thấy RefreshToken");
            }
            if (tokenDB.ExpiryDate < DateTime.Now)
            {
                throw new AppException(401, "Đăng nhập lại");
            }
            if (tokenDB.Invalidated)
            {
                throw new AppException(401, "Đăng nhập lại, Bị hacker tấn công");
            }
            if (tokenDB.Used)
            {
                await _refreshTokenRepository.Revoke(tokenDB);
                throw new AppException(401, "Đăng nhập lại, Bị hacker tấn công");
            }

            // đánh dấu đã dùng
            await _refreshTokenRepository.Used(tokenDB);

            var user = await _userRepository.FindByIdAsync(tokenDB.UserId);
            var roles = await _userRepository.GetRolesAsync(user);
            var AccessToken = _tokenService.CreateToken(user, roles);

            // lấy jwtId từ payload
            var jti = getJwtId(AccessToken);

            var RefreshToken = _tokenService.CreateRefreshToken(user.Id, jti);

            await _refreshTokenRepository.CreateTokenAsync(RefreshToken);
            return (AccessToken, RefreshToken.Token);
        }

        public string getJwtId(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(accessToken))
            {
                Console.WriteLine("Token lỗi hoặc không đúng định dạng JWT.");
                return null;
            }
            var jsonToken = handler.ReadJwtToken(accessToken);
            var jti = jsonToken.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;
            return jti;
        }

        public async Task Logout(string token)
        {
            var tokenDb = await _refreshTokenRepository.GetByTokenAsync(token);
            if (tokenDb == null)
            {
                throw new AppException(400, "RefreshToken Not Found");
            }
            await _refreshTokenRepository.Revoke(tokenDb);
        }

        public async Task LogoutAllDevices(string userId)
        {
            var rs = await _refreshTokenRepository.RevokeAllDevicesAsync(userId);
            if (!rs)
            {
                throw new AppException(500, "Có lỗi sever");
            }
        }
    }
}