using AppGroupCar.DTO.ResponseDTO.Bus.CompanyVehicleResponse;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Exceptions;
using AppGroupCar.DTO.RequestDTO.Bus.VehicleRequest;
using AppGroupCar.Repositories.BaseRepo;
using AutoMapper;
using AppGroupCar.Services.Mytoys.Photo;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Bus.CompanyVehicleServices
{
    public class CompanyVehicleService : ICompanyVehicleService
    {

        private readonly IBaseRepository<CompanyVehicle> _repo;
        private readonly IBaseRepository<VehicleSeatLayout> _seatLayoutRepo;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public CompanyVehicleService(
            IBaseRepository<CompanyVehicle> repo,
            IBaseRepository<VehicleSeatLayout> seatLayoutRepo,
            IMapper mapper,
            IPhotoService photoService)
        {
            _repo = repo;
            _seatLayoutRepo = seatLayoutRepo;
            _mapper = mapper;
            _photoService = photoService;
        }

        public async Task<CompanyVehicleDetailResponseDto?> GetByIdAsync(Guid id, Guid companyId)
        {
            var entity = await _repo.GetQueryable()
                .Include(x => x.Company)
                .Include(x => x.VehicleSeatLayout)
                .FirstOrDefaultAsync(x =>
                    x.CompanyVehicleId == id &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null);

            return entity == null ? null : _mapper.Map<CompanyVehicleDetailResponseDto>(entity);
        }

        public async Task<List<CompanyVehicleResponseDto>> GetByCompanyAsync(Guid companyId)
        {
            var entities = await _repo.GetQueryable()
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.DeleteAt == null)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<CompanyVehicleResponseDto>>(entities);
        }

        public async Task<CompanyVehicleDetailResponseDto> CreateAsync(Guid companyId, CreateCompanyVehicleRequest dto)
        {
            dto.PlateNumber = dto.PlateNumber.Trim().ToUpperInvariant();

            var isPlateNumberExists = await _repo.GetQueryable()
                .AnyAsync(x =>
                    x.PlateNumber == dto.PlateNumber &&
                    x.DeleteAt == null);

            if (isPlateNumberExists)
                throw new AppException(409, "Plate number already exists");

            var seatLayout = await _seatLayoutRepo.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == dto.SeatLayoutId && x.DeleteAt == null);

            if (seatLayout == null)
                throw new AppException(404, "Seat layout not found");

            string? vehicleImagePublicId = null;
            string? registrationPublicId = null;

            try
            {
                var vehicleUpload = await _photoService.UploadPhotoAsync(dto.UrlImage, "company-vehicles");
                if (vehicleUpload == null || vehicleUpload.Error != null)
                    throw new AppException(400, "Upload vehicle image failed: " + vehicleUpload?.Error?.Message);

                var registrationUpload = await _photoService.UploadPhotoAsync(dto.RegistrationDocumentImage, "company-vehicles");
                if (registrationUpload == null || registrationUpload.Error != null)
                    throw new AppException(400, "Upload registration document failed: " + registrationUpload?.Error?.Message);

                vehicleImagePublicId = vehicleUpload.PublicId;
                registrationPublicId = registrationUpload.PublicId;

                var entity = _mapper.Map<CompanyVehicle>(dto);
                entity.CompanyVehicleId = Guid.NewGuid();
                entity.CompanyId = companyId;
                entity.UrlImage = vehicleUpload.SecureUrl.AbsoluteUri;
                entity.RegistrationDocumentUrl = registrationUpload.SecureUrl.AbsoluteUri;

                await _repo.AddAsync(entity);
                await _repo.SaveChangesAsync();

                var createdEntity = await _repo.GetQueryable()
                    .Include(x => x.Company)
                    .Include(x => x.VehicleSeatLayout)
                    .FirstOrDefaultAsync(x => x.CompanyVehicleId == entity.CompanyVehicleId)
                    ?? throw new AppException(500, "Created company vehicle could not be loaded");

                return _mapper.Map<CompanyVehicleDetailResponseDto>(createdEntity);
            }
            catch
            {
                if (!string.IsNullOrEmpty(vehicleImagePublicId))
                    await _photoService.DeletePhotoAsync(vehicleImagePublicId);

                if (!string.IsNullOrEmpty(registrationPublicId))
                    await _photoService.DeletePhotoAsync(registrationPublicId);

                throw;
            }
        }

        public async Task<CompanyVehicleResponseDto> UpdateAsync(Guid id, Guid companyId, UpdateCompanyVehicleRequest dto)
        {
            var entity = await _repo.GetQueryable()
                .FirstOrDefaultAsync(x =>
                    x.CompanyVehicleId == id &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null);

            if (entity == null)
                throw new AppException(404, "Company vehicle not found");

            if (dto.SeatLayoutId.HasValue)
            {
                var seatLayout = await _seatLayoutRepo.GetQueryable()
                    .FirstOrDefaultAsync(x => x.Id == dto.SeatLayoutId.Value && x.DeleteAt == null);

                if (seatLayout == null)
                    throw new AppException(404, "Seat layout not found");
            }

            var oldVehicleImageUrl = entity.UrlImage;
            var oldRegistrationDocumentUrl = entity.RegistrationDocumentUrl;

            string? newVehicleImagePublicId = null;
            string? newRegistrationPublicId = null;

            try
            {
                if (dto.SeatLayoutId.HasValue)
                    entity.SeatLayoutId = dto.SeatLayoutId.Value;

                if (dto.SeatCapacity.HasValue)
                    entity.SeatCapacity = dto.SeatCapacity.Value;

                if (dto.VehicleType.HasValue)
                {
                    if (!Enum.IsDefined(typeof(Entities.Market.VehicleType), dto.VehicleType.Value))
                        throw new AppException(400, "Invalid vehicle type");

                    entity.VehicleType = (Entities.Market.VehicleType)dto.VehicleType.Value;
                }

                if (dto.Status.HasValue)
                {
                    if (!Enum.IsDefined(typeof(Entities.Market.VehicleStatus), dto.Status.Value))
                        throw new AppException(400, "Invalid vehicle status");

                    entity.Status = (Entities.Market.VehicleStatus)dto.Status.Value;
                }

                if (dto.UrlImage != null)
                {
                    var vehicleUpload = await _photoService.UploadPhotoAsync(dto.UrlImage, "company-vehicles");

                    if (vehicleUpload == null || vehicleUpload.Error != null)
                        throw new AppException(400, "Upload vehicle image failed: " + vehicleUpload?.Error?.Message);

                    entity.UrlImage = vehicleUpload.SecureUrl.AbsoluteUri;
                    newVehicleImagePublicId = vehicleUpload.PublicId;
                }

                if (dto.RegistrationDocumentImage != null)
                {
                    var registrationUpload = await _photoService.UploadPhotoAsync(dto.RegistrationDocumentImage, "company-vehicles");

                    if (registrationUpload == null || registrationUpload.Error != null)
                        throw new AppException(400, "Upload registration document failed: " + registrationUpload?.Error?.Message);

                    entity.RegistrationDocumentUrl = registrationUpload.SecureUrl.AbsoluteUri;
                    newRegistrationPublicId = registrationUpload.PublicId;
                }

                _repo.Update(entity);
                await _repo.SaveChangesAsync();
            }
            catch
            {
                if (!string.IsNullOrEmpty(newVehicleImagePublicId))
                    await _photoService.DeletePhotoAsync(newVehicleImagePublicId);

                if (!string.IsNullOrEmpty(newRegistrationPublicId))
                    await _photoService.DeletePhotoAsync(newRegistrationPublicId);

                throw;
            }

            if (dto.UrlImage != null && !string.IsNullOrEmpty(oldVehicleImageUrl))
            {
                var oldVehicleImagePublicId = _photoService.GetPublicIdFromUrl(oldVehicleImageUrl);
                if (!string.IsNullOrEmpty(oldVehicleImagePublicId))
                    await _photoService.DeletePhotoAsync(oldVehicleImagePublicId);
            }

            if (dto.RegistrationDocumentImage != null && !string.IsNullOrEmpty(oldRegistrationDocumentUrl))
            {
                var oldRegistrationPublicId = _photoService.GetPublicIdFromUrl(oldRegistrationDocumentUrl);
                if (!string.IsNullOrEmpty(oldRegistrationPublicId))
                    await _photoService.DeletePhotoAsync(oldRegistrationPublicId);
            }

            return _mapper.Map<CompanyVehicleResponseDto>(entity);
        }

        public async Task DeleteAsync(Guid id, Guid companyId)
        {
            var entity = await _repo.GetQueryable()
                .FirstOrDefaultAsync(x =>
                    x.CompanyVehicleId == id &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null);

            if (entity == null)
                throw new AppException(404, "Company vehicle not found");

            entity.DeleteAt = DateTime.UtcNow;
            _repo.Update(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task<string> GetSeatLayoutJsonByVehicleAsync(Guid companyVehicleId, Guid companyId)
        {
            var entity = await _repo.GetQueryable()
                .Include(x => x.VehicleSeatLayout)
                .FirstOrDefaultAsync(x =>
                    x.CompanyVehicleId == companyVehicleId &&
                    x.CompanyId == companyId &&
                    x.DeleteAt == null);

            if (entity == null)
                throw new AppException(404, "Company vehicle not found");

            if (entity.VehicleSeatLayout == null || entity.VehicleSeatLayout.DeleteAt != null)
                throw new AppException(404, "Vehicle seat layout not found");

            return entity.VehicleSeatLayout.LayoutJson;
        }
    }
}
