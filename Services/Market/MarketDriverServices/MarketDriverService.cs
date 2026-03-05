using AppGroupCar.DTO.RequestDTO.Market.MarketDriver;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Services.Mytoys.Photo;
using AutoMapper;

namespace AppGroupCar.Services.Market.MarketDriverServices
{
    public class MarketDriverService : IMarketDriverService
    {
        private readonly IBaseRepository<Driver> _repo;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public MarketDriverService(IBaseRepository<Driver> repo, IMapper mapper, IPhotoService photoService)
        {
            _repo = repo;
            _mapper = mapper;
            _photoService = photoService;
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
