using AppGroupCar.DTO.RequestDTO.Market.Vehicle;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AppGroupCar.Services.Mytoys.Photo;
using AutoMapper;

namespace AppGroupCar.Services.Market.VehicleServices
{
    public class VehicleService : IVehicleService
    {
        private readonly IBaseRepository<Vehicle> _vehicleRepo;
        private readonly IBaseRepository<Driver> _driverRepo;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        public VehicleService(
            IBaseRepository<Vehicle> vehicleRepo,
            IBaseRepository<Driver> driverRepo,
            IPhotoService photoService,
            IMapper mapper)
        {
            _vehicleRepo = vehicleRepo;
            _driverRepo = driverRepo;
            _photoService = photoService;
            _mapper = mapper;
        }

        public async Task<VehicleDTO> CreateAsync(Guid driverId, CreateVehicleDTO dto)
        {
            var driver = await _driverRepo.GetByIdAsync(driverId);
            if (driver == null)
                throw new AppException(404, "Driver not found");

            var existingVehicle = await _vehicleRepo.FindAsync(
                v => v.PlateNumber == dto.PlateNumber && v.DeleteAt == null,
                1, 1
            );
            if (existingVehicle.Any())
                throw new AppException(400, "Plate number already exists");

            var vehicle = _mapper.Map<Vehicle>(dto);
            vehicle.DriverId = driverId;

            if (dto.UrlImage != null && dto.UrlImage.Length > 0)
            {
                var uploadResult = await _photoService.AddPhotoAsync(dto.UrlImage);
                if (uploadResult.Error != null)
                    throw new AppException(400, "Cloudinary upload failed: " + uploadResult.Error.Message);
                vehicle.UrlImage = uploadResult.SecureUrl.AbsoluteUri;
            }

            if (dto.RegistrationDocumentUrl != null && dto.RegistrationDocumentUrl.Length > 0)
            {
                var uploadResult = await _photoService.AddPhotoAsync(dto.RegistrationDocumentUrl);
                if (uploadResult.Error != null)
                    throw new AppException(400, "Cloudinary upload failed: " + uploadResult.Error.Message);
                vehicle.RegistrationDocumentUrl = uploadResult.SecureUrl.AbsoluteUri;
            }

            await _vehicleRepo.AddAsync(vehicle);
            await _vehicleRepo.SaveChangesAsync();

            return _mapper.Map<VehicleDTO>(vehicle);
        }

        public async Task<IEnumerable<VehicleDTO>> GetByDriverIdAsync(Guid driverId)
        {
            var driver = await _driverRepo.GetByIdAsync(driverId);
            if (driver == null)
                throw new AppException(404, "Driver not found");

            var vehicles = await _vehicleRepo.FindAsync(
                v => v.DriverId == driverId && v.DeleteAt == null,
                1,
                int.MaxValue
            );

            return _mapper.Map<IEnumerable<VehicleDTO>>(vehicles);
        }

        public async Task<VehicleDTO> GetByIdAsync(Guid id)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(id);
            if (vehicle == null || vehicle.DeleteAt != null)
                throw new AppException(404, "Vehicle not found");

            return _mapper.Map<VehicleDTO>(vehicle);
        }

        public async Task UpdateAsync(Guid id, Guid driverId, UpdateVehicleDTO dto)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(id);
            if (vehicle == null || vehicle.DeleteAt != null)
                throw new AppException(404, "Vehicle not found");

            // quyền sở hữu
            if (vehicle.DriverId != driverId)
                throw new AppException(403, "You don't have permission to update this vehicle");


            if (!string.IsNullOrEmpty(dto.PlateNumber) && dto.PlateNumber != vehicle.PlateNumber)
            {
                var existingVehicle = await _vehicleRepo.FindAsync(
                    v => v.PlateNumber == dto.PlateNumber && v.DeleteAt == null && v.Id != id,
                    1, 1
                );
                if (existingVehicle.Any())
                    throw new AppException(400, "Plate number already exists");
            }

            _mapper.Map(dto, vehicle);

            if (dto.UrlImage != null && dto.UrlImage.Length > 0)
            {
                var uploadResult = await _photoService.AddPhotoAsync(dto.UrlImage);
                if (uploadResult.Error != null)
                    throw new AppException(400, "Cloudinary upload failed: " + uploadResult.Error.Message);
                vehicle.UrlImage = uploadResult.SecureUrl.AbsoluteUri;
            }

            if (dto.RegistrationDocumentUrl != null && dto.RegistrationDocumentUrl.Length > 0)
            {
                var uploadResult = await _photoService.AddPhotoAsync(dto.RegistrationDocumentUrl);
                if (uploadResult.Error != null)
                    throw new AppException(400, "Cloudinary upload failed: " + uploadResult.Error.Message);
                vehicle.RegistrationDocumentUrl = uploadResult.SecureUrl.AbsoluteUri;
            }

            vehicle.LastUpdateAt = DateTime.UtcNow;
            _vehicleRepo.Update(vehicle);
            await _vehicleRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, Guid driverId)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(id);
            if (vehicle == null || vehicle.DeleteAt != null)
                throw new AppException(404, "Vehicle not found");

            if (vehicle.DriverId != driverId)
                throw new AppException(403, "You don't have permission to delete this vehicle");

            vehicle.DeleteAt = DateTime.UtcNow;
            _vehicleRepo.Update(vehicle);
            await _vehicleRepo.SaveChangesAsync();
        }
    }
}
