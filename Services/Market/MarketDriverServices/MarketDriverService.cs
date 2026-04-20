using AppGroupCar.Data;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Market.MarketDriver;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Identity;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Services.Auth.RoleServices;
using AppGroupCar.Services.Mytoys.Photo;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Transactions;

namespace AppGroupCar.Services.Market.MarketDriverServices
{
    public class MarketDriverService : IMarketDriverService
    {
        private readonly IBaseRepository<Driver> _repo;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly UserManager<ApplicationUser> _user;
        private readonly ApplicationDbContext _dbContext;
        public MarketDriverService(IBaseRepository<Driver> repo, IMapper mapper, IPhotoService photoService, UserManager<ApplicationUser> user, ApplicationDbContext dbContext)
        {
            _repo = repo;
            _mapper = mapper;
            _photoService = photoService;
            _user = user;
            _dbContext = dbContext;
        }

        public async Task ApprovalDriverAsync(Guid id)
        {
            // 3. Lấy Strategy từ DbContext để dập tắt cái lỗi Retry
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                // Nhớ using System.Transactions; ở đầu file nhé!
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                string userId = id.ToString();

                var user = await _user.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new AppException(404, "Người dùng không tồn tại");
                }

                var rs = await _user.AddToRoleAsync(user, "Driver");
                if (!rs.Succeeded)
                {
                    throw new AppException(500, "Thêm vai trò thất bại");
                }

                var driver = await _repo.GetByIdAsync(id) ?? throw new AppException(404, "Tài xế không tồn tại");

                driver.VerificationStatus = DriverVerificationStatus.Approved;
                _repo.Update(driver);
                await _repo.SaveChangesAsync();

                // Chạy qua hết thì chốt đơn!
                scope.Complete();
            });
        }

        public async Task<Driver> CreatAsync(Guid id, CreateDriverRequestDTO request)
        {
            string? imgUrl = null;

            if (request.LicenseDocumentImg != null)
            {
                var uploadResult = await _photoService.AddPhotoAsync(request.LicenseDocumentImg);
                if (uploadResult.Error != null)
                    throw new AppException(400, "Cloudinary upload failed: " + uploadResult.Error.Message);

                imgUrl = uploadResult.SecureUrl.AbsoluteUri;
            }

            var driver = _mapper.Map<Driver>(request);
            driver.Id = id;
            driver.VerificationStatus = DriverVerificationStatus.Pending;
            driver.LicenseDocumentUrl = imgUrl;

            await _repo.AddAsync(driver);
            await _repo.SaveChangesAsync();
            return driver;
        }

        public async Task DeleteAsync(Guid id)
        {
            var driver = await _repo.GetByIdAsync(id);
            if (driver == null || driver?.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");

            }
            _repo.Delete(driver);
            await _repo.SaveChangesAsync();
            return;
        }

        public async Task<Driver> GetByIdAsync(Guid id)
        {
            var driver = await _repo.GetByIdAsync(id);
            if (driver == null || driver?.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            return driver;
        }

        public Task<IEnumerable<Driver>> GetMarketDriversAsync(DriverParameters request)
        {
            throw new NotImplementedException();
        }

        public async Task<Driver> UpdateAsync(Guid id, UpdateDriverRequestDTO request)
        {
            var driver = await _repo.GetByIdAsync(id);
            if (driver == null || driver?.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            string? imgUrl = null;

            if (request.LicenseDocumenImg != null)
            {
                var uploadResult = await _photoService.AddPhotoAsync(request.LicenseDocumenImg);
                if (uploadResult.Error != null)
                    throw new AppException(400, "Cloudinary upload failed: " + uploadResult.Error.Message);

                imgUrl = uploadResult.SecureUrl.AbsoluteUri;
            }
            _mapper.Map(request, driver);
            driver.LicenseDocumentUrl = imgUrl;
            _repo.Update(driver);
            await _repo.SaveChangesAsync();
            return driver;
        }
    }
}
