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
        private readonly IBaseRepository<SharedRideOfferDetail> _repoOfferDetail;
        private readonly IBaseRepository<OfferRoutePoint> _repoOfferRoutePoint;
        private readonly IMapper _mapper;

        public OfferService(
            IBaseRepository<Offer> repo,
            IBaseRepository<Vehicle> vehicleRepo,
            IBaseRepository<SharedRideOfferDetail> repoOfferDetail,
            IBaseRepository<OfferRoutePoint> repoOfferRoutePoint,
            IMapper mapper)
        {
            _repo = repo;
            _vehicleRepo = vehicleRepo;
            _mapper = mapper;
            _repoOfferDetail = repoOfferDetail;
            _repoOfferRoutePoint = repoOfferRoutePoint;
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
            var offers = await _repo.GetQueryable().Select(o => new GetOfferDTO
            {
                Id = o.Id,
                DriverId = o.DriverId,
                DriverName = o.Driver.Name,
                VehicleId = o.VehicleId,
                VehicleName = o.Vehicle.Brand,
                ServiceType = o.ServiceType,
                DepartureTime = o.DepartureTime,
                BasePrice = o.BasePrice,
                Status = o.Status,
                TotalSeats = o.SharedRideDetail.TotalSeats,
                AvailableSeats = o.SharedRideDetail.AvailableSeats
            }).ToListAsync();

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
                        .Include(o => o.SharedRideDetail);
        }

        public async Task<GetOfferDTO> CreateWithDetailAsync(Guid driverId, CreateOfferWithDetailRequestDTO request)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId);
            if (vehicle == null || vehicle.DriverId != driverId)
            {
                throw new AppException(400, "Vehicle không tồn tại hoặc không thuộc về bạn");
            }
            if (request.DepartureTime < DateTime.Now)
            {
                throw new AppException(400, "Thời gian không hợp lệ");
            }
            if (request.OfferRoutePoints.Count() < 2)
            {
                throw new AppException(400, "Offer phải có ít nhất 2 Route Point");
            }

            var recentOffer = await _repo.GetQueryable()
                                            .Where(o => o.DriverId == driverId && o.DepartureTime <= DateTimeOffset.UtcNow && o.Status == OfferStatus.Active)
                                            .OrderByDescending(o => o.DepartureTime)
                                            .FirstOrDefaultAsync();
            if (recentOffer != null)
            {
                throw new AppException(400, "Bạn đang có Offer hoạt động. Vui lòng hoàn thành Offer cũ để có thể tạo mới");
            }

            var offer = _mapper.Map<Offer>(request);
            offer.Id = Guid.NewGuid();
            offer.DriverId = driverId;

            var newOfferDetail = new SharedRideOfferDetail
            {
                Id = offer.Id,
                AvailableSeats = vehicle.SeatCapacity,
                TotalSeats = vehicle.SeatCapacity,
            };

            foreach (var item in request.OfferRoutePoints)
            {
                var newOfferRoutePoint = new OfferRoutePoint
                {
                    Id = Guid.NewGuid(),
                    OfferId = offer.Id,
                    Sequence = item.Sequence,
                    LocationId = item.LocationId
                };
                await _repoOfferRoutePoint.AddAsync(newOfferRoutePoint);
            }

            await _repo.AddAsync(offer);

            await _repoOfferDetail.AddAsync(newOfferDetail);

            await _repo.SaveChangesAsync();


            var offerResponse = _mapper.Map<GetOfferDTO>(offer);
            offerResponse.TotalSeats = vehicle.SeatCapacity;
            offerResponse.AvailableSeats = vehicle.SeatCapacity;

            return offerResponse;
        }
    }
}