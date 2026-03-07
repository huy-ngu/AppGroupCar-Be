using AppGroupCar.AutoMapper;
using AppGroupCar.Data;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Identity;
using AppGroupCar.Helpers;
using AppGroupCar.Middleware;
using AppGroupCar.Repositories.Auth.RefreshTokensRepo;
using AppGroupCar.Repositories.Auth.UserRepo;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Services.Auth.AuthServices;
using AppGroupCar.Services.Auth.RoleServices;
using AppGroupCar.Services.Auth.TokenServices;
using AppGroupCar.Services.Auth.UserServices;
using AppGroupCar.Services.Market.MarketDriverServices;
using AppGroupCar.Services.Market.RideRequestServices;
using AppGroupCar.Services.Market.ShipmentRequestServices;
using AppGroupCar.Services.Mytoys.LocationServices;
using AppGroupCar.Services.Mytoys.Photo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Text;

namespace AppGroupCar
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();
            try
            {
                Log.Information("Đang khởi động ứng dụng...");


                var builder = WebApplication.CreateBuilder(args);

                // thêm log
                builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

                // Add services to the container.

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    // add JWT auth to swagger
                    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJI...\""
                    });
                    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });
                });

                //var connectionString =
                //    builder.Configuration.GetConnectionString("AzureConnection")
                //    ?? throw new InvalidOperationException("AzureConnection is missing");
                var connectionString =
                    builder.Configuration.GetConnectionString("DefaultConnection");

                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        connectionString,
                        sql =>
                        {
                            sql.UseNetTopologySuite();
                            sql.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(10),
                                errorNumbersToAdd: null
                            );
                        }
                    )
                );

                builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    // Password
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;

                    // Lockout
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

                    // User
                    options.User.RequireUniqueEmail = true;

                    // SignIn
                    options.SignIn.RequireConfirmedEmail = false;
                })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


                var jwtSettings = builder.Configuration.GetSection("Jwt");
                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                })
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Google:ClientId"]!;
                    options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
                });

                // repositories
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
                builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

                // services
                builder.Services.AddScoped<ITokenService, TokenService>();
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<IRoleService, RoleService>();
                builder.Services.AddScoped<IRideRequestService, RideRequestService>();
                builder.Services.AddScoped<IShipmentRequestServices, ShipmentRequestServices>();
                builder.Services.AddScoped<LocationService>();
                builder.Services.AddScoped<IPhotoService, PhotoService>();
                builder.Services.AddScoped<IMarketDriverService, MarketDriverService>();

                // Map section trong appsettings vào class CloudinarySettings
                builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

                // authoriztion
                builder.Services.AddAuthorization();

                //Đăng ký AutoMapper
                builder.Services.AddAutoMapper(cfg => { },
                typeof(ShipmentRequestMapping).Assembly);


                var app = builder.Build();

                //khởi tạo role mỗi khi chạy lần đầu
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        // Lấy các Manager từ hệ thống DI
                        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                        // Chạy hàm Seed
                        await IdentityDataInitializer.SeedDataAsync(userManager, roleManager);
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "Đã xảy ra lỗi khi Seed dữ liệu.");
                    }
                }

                app.UseSerilogRequestLogging();

                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseHttpsRedirection();

                // add exception middleware
                app.UseMiddleware<ExceptionMiddleware>();

                app.UseAuthentication();
                app.UseAuthorization();


                app.MapControllers();

                app.Run();

            }
            catch (Exception ex)
            {
                // Bắt mọi lỗi xảy ra làm sập app lúc mới chạy
                Log.Fatal(ex, "Ứng dụng chết ngắc ngay lúc khởi động!");
            }

        }
    }
    public static class IdentityDataInitializer
    {
        public static async Task SeedDataAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            // 1. Khởi tạo Roles
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole(roleName));
                }
            }

            // 2. Khởi tạo tài khoản Admin mặc định
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Administrator" // Trường custom nếu bạn có
                };

                // Tạo User với mật khẩu mặc định
                var result = await userManager.CreateAsync(newAdmin, "Admin@123");

                if (result.Succeeded)
                {
                    // Gán quyền Admin cho tài khoản vừa tạo
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}
