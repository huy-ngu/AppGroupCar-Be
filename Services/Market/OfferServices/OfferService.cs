using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppGroupCar.Services.Market.OfferServices
{
    public class OfferService : IOfferService
    {
        private readonly IBaseRepository<Offer> _repo;
        private readonly IBaseRepository<Vehicle> _vehicleRepo;
        private readonly IMapper _mapper;

        public OfferService(IBaseRepository<Offer> repo, IBaseRepository<Vehicle> vehicleRepo, IMapper mapper)
        {
            _repo = repo;
            _vehicleRepo = vehicleRepo;
            _mapper = mapper;
        }

        public async Task<Offer> CreateAsync(Guid driverId, CreateOfferRequestDTO request)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId);
            if (vehicle == null || vehicle.DriverId != driverId || vehicle.DeleteAt != null)
            {
                throw new AppException(400, "Vehicle không tồn tại hoặc không thuộc về bạn");
            }

            var offer = _mapper.Map<Offer>(request);
            offer.DriverId = driverId;
            await _repo.AddAsync(offer);
            await _repo.SaveChangesAsync();
            return offer;
        }

        public async Task DeleteAsync(Guid driverId, Guid id)
        {
            var offer = await _repo.GetByIdAsync(id);
            if (offer == null || offer.DeleteAt != null || offer.DriverId != driverId)
            {
                throw new AppException(404, "Không tìm thấy bản ghi hoặc bạn không có quyền xóa");
            }
            _repo.Delete(offer);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<Offer>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();
            return data.Where(x => x.DeleteAt == null);
        }

        public async Task<IEnumerable<Offer>> GetMyOffersAsync(Guid driverId)
        {
            var data = await _repo.GetAllAsync();
            return data.Where(x => x.DeleteAt == null && x.DriverId == driverId);
        }

        public async Task<Offer> GetByIdAsync(Guid id)
        {
            var offer = await _repo.GetByIdAsync(id);
            if (offer == null || offer.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }
            return offer;
        }

        public async Task<Offer> UpdateAsync(Guid driverId, Guid id, UpdateOfferRequestDTO request)
        {
            var offer = await _repo.GetByIdAsync(id);
            if (offer == null || offer.DeleteAt != null || offer.DriverId != driverId)
            {
                throw new AppException(404, "Không tìm thấy bản ghi hoặc bạn không có quyền cập nhật");
            }

            if (request.VehicleId.HasValue)
            {
                var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId.Value);
                if (vehicle == null || vehicle.DriverId != driverId || vehicle.DeleteAt != null)
                {
                    throw new AppException(400, "Vehicle không tồn tại hoặc không thuộc về bạn");
                }
                offer.VehicleId = (Guid)request.VehicleId;
            }

            _mapper.Map(request, offer);

            _repo.Update(offer);
            await _repo.SaveChangesAsync();
            return offer;
        }
    }
}
