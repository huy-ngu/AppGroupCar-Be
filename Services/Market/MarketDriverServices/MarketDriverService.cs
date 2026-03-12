using AppGroupCar.Data;
using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.MarketDriver;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.Auth.UserRepo;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Services.Mytoys.Photo;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Market.MarketDriverServices
{
    public class MarketDriverService : IMarketDriverService
    {
        private readonly IBaseRepository<Driver> _repo;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public MarketDriverService(IBaseRepository<Driver> repo, IMapper mapper, IPhotoService photoService, IUserRepository userRepository, ApplicationDbContext context)
        {
            _repo = repo;
            _mapper = mapper;
            _photoService = photoService;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<Driver> CreatAsync(Guid id, string fullName, CreateDriverRequestDTO request)
        {
            string? imgUrl = null;
            string? imagePublicId = null;

            var driverExist = await _repo.GetByIdAsync(id);

            if (driverExist != null)
            {
                throw new AppException(400, "User này đã là Driver");
            }

            // 1. GIAI ĐOẠN UPLOAD CLOUD
            if (request.LicenseDocumentImg != null)
            {
                var uploadResult = await _photoService.AddPhotoAsync(request.LicenseDocumentImg);
                if (uploadResult.Error != null)
                    throw new AppException(400, "Cloudinary upload failed: " + uploadResult.Error.Message);

                imgUrl = uploadResult.SecureUrl.AbsoluteUri;
                // Bắt buộc lấy PublicId để nếu DB lưu xịt thì còn có cái mà xóa
                imagePublicId = uploadResult.PublicId;
            }

            var driver = _mapper.Map<Driver>(request);
            driver.Id = id;
            driver.Name = fullName;
            driver.VerificationStatus = DriverVerificationStatus.Pending;
            driver.LicenseDocumentUrl = imgUrl;

            // 2. BỌC TOÀN BỘ LOGIC VÀO TRY-CATCH NGOÀI CÙNG ĐỂ XỬ LÝ LỖI PHÂN TÁN
            try
            {
                var strategy = _context.Database.CreateExecutionStrategy();

                // Thực thi Database Transaction với Retry Strategy
                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        // Bước 1: Thêm driver vào EF Core
                        await _repo.AddAsync(driver);

                        // Bước 2: Thêm Role bằng Identity
                        var user = await _userRepository.FindByIdAsync(id);
                        var result = await _userRepository.AddToRolesAsync(user, new[] { "Driver" });

                        if (!result.Succeeded)
                        {
                            var errors = result.Errors.Select(e => e.Description);
                            throw new AppException(400, "Failed", new { errors });
                        }

                        // Bước 3: Lưu DB
                        await _repo.SaveChangesAsync();

                        // Bước 4: Chốt sổ DB
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        // Rollback DB khi có lỗi
                        await transaction.RollbackAsync();

                        // CẤM TUYỆT ĐỐI THROW LỖI MỚI Ở ĐÂY!
                        // Phải "throw;" để giữ nguyên lỗi gốc cho Strategy biết đường mà Retry
                        throw;
                    }
                });

                return driver;
            }
            catch (Exception ex)
            {
                // 3. BƯỚC DỌN RÁC (COMPENSATING TRANSACTION)
                // Code lọt xuống tận đây nghĩa là DB đã thất bại hoàn toàn (Retry cũng vô phương cứu chữa)

                // Kiểm tra xem lúc nãy đã upload ảnh lên Cloudinary thành công chưa?
                // Nếu rồi thì phải gọi API xóa nó đi để tránh rác!
                if (!string.IsNullOrEmpty(imagePublicId))
                {
                    try
                    {
                        await _photoService.DeletePhotoAsync(imagePublicId);
                    }
                    catch (Exception cleanupEx)
                    {
                        // Ở đây nếu xóa ảnh cũng lỗi thì log lại thôi, đừng throw đè mất lỗi gốc của DB
                        // Ghi log ở đây (ví dụ: _logger.LogError(cleanupEx, "Lỗi khi xóa ảnh rác trên Cloudinary");)
                    }
                }

                // Tới đây bạn có thể ném ra lỗi để Frontend biết mà hiển thị
                // Nếu lỗi gốc là AppException (do vụ AddToRolesAsync ném ra) thì quăng lại nó
                if (ex is AppException)
                {
                    throw;
                }
                throw new AppException(500, "Lỗi hệ thống khi lưu dữ liệu", ex.Message);
            }
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

        public async Task<PagedResult<Driver>> FilterAsync(MarketDriverParameters param)
        {
            var data = await _repo.FindAndFilter(
                predicate: r =>
                    //  Lọc điểm đi (nếu null/rỗng thì bỏ qua điều kiện này)
                    (string.IsNullOrEmpty(param.fullName) || r.Name.Contains(param.fullName)) &&
                    (param.status == null) || (r.VerificationStatus == param.status) &&
                    r.DeleteAt == null,

                orderBy: q => q.OrderByDescending(r => r.CreatedAt),
                pageNumber: param.PageNumber,
                pageSize: param.PageSize
            );
            return data;
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
                driver.LicenseDocumentUrl = imgUrl;
            }
            _mapper.Map(request, driver);
            _repo.Update(driver);
            await _repo.SaveChangesAsync();
            return driver;
        }
    }
}
