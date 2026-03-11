using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        // Chú ý: Đã loại bỏ _driverRepo và _sharedRideRepo vì không cần gọi lắt nhắt nữa
        private readonly IMapper _mapper;

        public OfferService(
            IBaseRepository<Offer> repo,
            IBaseRepository<Vehicle> vehicleRepo,
            IMapper mapper)
        {
            _repo = repo;
            _vehicleRepo = vehicleRepo;
            _mapper = mapper;
        }

        public async Task<GetOfferDTO> CreateAsync(Guid driverId, CreateOfferRequestDTO request)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId);
            if (vehicle == null || vehicle.DriverId != driverId || vehicle.DeleteAt != null)
            {
                throw new AppException(400, "Vehicle không tồn tại hoặc không thuộc về bạn");
            }

            // Dùng AutoMapper để chuyển Request thành Entity
            var offer = _mapper.Map<Offer>(request);
            offer.DriverId = driverId;

            await _repo.AddAsync(offer);
            await _repo.SaveChangesAsync();

            // Lấy lại dữ liệu vừa tạo kèm theo các bảng liên quan (Eager Loading)
            var createdOffer = await GetOfferWithRelationsQuery()
                                .FirstOrDefaultAsync(x => x.Id == offer.Id);

            // Tự động map sang DTO
            return _mapper.Map<GetOfferDTO>(createdOffer);
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

        public async Task<IEnumerable<GetOfferDTO>> GetAllAsync()
        {
            // Truy vấn database, join luôn các bảng cần thiết, chỉ lấy record chưa bị xóa
            var offers = await GetOfferWithRelationsQuery()
                                .Where(x => x.DeleteAt == null)
                                .ToListAsync();

            // Chỉ 1 dòng code gọn gàng để map cả một List
            return _mapper.Map<IEnumerable<GetOfferDTO>>(offers);
        }

        public async Task<IEnumerable<GetOfferDTO>> GetMyOffersAsync(Guid driverId)
        {
            // Truy vấn kết hợp lọc dữ liệu ngay tại DB (tiết kiệm bộ nhớ server)
            var offers = await GetOfferWithRelationsQuery()
                                .Where(x => x.DeleteAt == null && x.DriverId == driverId)
                                .ToListAsync();

            return _mapper.Map<IEnumerable<GetOfferDTO>>(offers);
        }

        public async Task<GetOfferDTO> GetByIdAsync(Guid id)
        {
            var offer = await GetOfferWithRelationsQuery()
                                .FirstOrDefaultAsync(x => x.Id == id);

            if (offer == null || offer.DeleteAt != null)
            {
                throw new AppException(404, "Không tìm thấy bản ghi");
            }

            return _mapper.Map<GetOfferDTO>(offer);
        }

        public async Task<GetOfferDTO> UpdateAsync(Guid driverId, Guid id, UpdateOfferRequestDTO request)
        {
            // Khi update, ta cũng nên include luôn các bảng để map DTO trả về sau đó
            var offer = await GetOfferWithRelationsQuery()
                                .FirstOrDefaultAsync(x => x.Id == id);

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
                offer.Vehicle = vehicle; // Gán để tí trả về map DTO không bị lỗi
            }

            // Map đè dữ liệu từ request vào offer hiện tại
            _mapper.Map(request, offer);

            _repo.Update(offer);
            await _repo.SaveChangesAsync();

            return _mapper.Map<GetOfferDTO>(offer);
        }

        // --- HÀM HỖ TRỢ DÙNG CHUNG ---
        // Giúp tập trung logic Include bảng liên quan vào 1 chỗ để tránh lặp code
        private IQueryable<Offer> GetOfferWithRelationsQuery()
        {
            // Yêu cầu IBaseRepository phải có phương thức GetQueryable()
            return _repo.GetQueryable()
                        .Include(o => o.Driver)
                        .Include(o => o.Vehicle)
                        // Giả sử 1 Offer có 1 SharedRideDetail, bạn map khóa ngoại chuẩn trong Entity nhé
                        .Include(o => o.SharedRideDetail);
        }
    }
}