using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.DTO.ResponseDTO.Market.Offer;
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
        private readonly IBaseRepository<Booking> _bookingRepo;
        private readonly IMapper _mapper;

        public OfferService(
            IBaseRepository<Offer> repo,
            IBaseRepository<Vehicle> vehicleRepo,
            IBaseRepository<SharedRideOfferDetail> repoOfferDetail,
            IBaseRepository<OfferRoutePoint> repoOfferRoutePoint,
            IBaseRepository<Booking> bookingRepo,
            IMapper mapper)
        {
            _repo = repo;
            _vehicleRepo = vehicleRepo;
            _mapper = mapper;
            _repoOfferDetail = repoOfferDetail;
            _repoOfferRoutePoint = repoOfferRoutePoint;
            _bookingRepo = bookingRepo;
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

        public async Task<GetOfferDTO> UpdateAsync(Guid driverId, UpdateOfferRequestDTO request)
        {
            // Khi update, ta cũng nên include luôn các bảng để map DTO trả về sau đó
            var offer = await GetOfferWithRelationsQuery()
                                .FirstOrDefaultAsync(x => x.Id == request.Id);

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
                offer.Vehicle = vehicle;
            }

            // Map đè dữ liệu từ request vào offer hiện tại
            _mapper.Map(request, offer);

            _repo.Update(offer);
            await _repo.SaveChangesAsync();

            return _mapper.Map<GetOfferDTO>(offer);
        }

        private IQueryable<Offer> GetOfferWithRelationsQuery()
        {
            // Yêu cầu IBaseRepository phải có phương thức GetQueryable()
            return _repo.GetQueryable()
                        .Include(o => o.Driver)
                        .Include(o => o.Vehicle)
                        .Include(o => o.SharedRideDetail);
        }

        public async Task<GetOfferDTO> CreateWithDetailAsync(Guid driverId, CreateOfferSharedRidelRequestDTO request)
        {
            var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId);
            if (vehicle == null || vehicle.DriverId != driverId)
            {
                throw new AppException(404, "Vehicle không tồn tại hoặc không thuộc về bạn");
            }
            if(vehicle.Status != VehicleStatus.Active)
            {
                throw new AppException(400, "Phương tiện không đủ điều kiện, Vui lòng chọn phương tiện đang hoạt động");
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
                                            .AsNoTracking()
                                            .Where(o => o.DriverId == driverId && o.Status == OfferStatus.Active && o.DeleteAt == null)
                                            .OrderByDescending(o => o.DepartureTime)
                                            .FirstOrDefaultAsync();
            if (recentOffer != null)
            {
                throw new AppException(400, "Bạn đang có Offer hoạt động. Vui lòng hoàn thành Offer cũ để có thể tạo mới");
            }

            var offer = _mapper.Map<Offer>(request);
            offer.Id = Guid.NewGuid();
            offer.DriverId = driverId;
            offer.ServiceType = ServiceType.Shared;
            offer.Status = OfferStatus.Active;

            var newOfferDetail = new SharedRideOfferDetail
            {
                Id = offer.Id,
                AvailableSeats = vehicle.SeatCapacity,
                TotalSeats = vehicle.SeatCapacity,
            };
            var newRoutePoints = request.OfferRoutePoints
                 .Select((item, index) => new OfferRoutePoint
                 {
                     Id = Guid.NewGuid(),
                     OfferId = offer.Id,
                     Sequence = index + 1,
                     LocationId = item.LocationId,
                     PickupAllowed = item.PickupAllowed ?? true,
                     DropoffAllowed = item.DropoffAllowed ?? true
                 })
                 .ToList();
            await _repoOfferRoutePoint.AddRangeAsync(newRoutePoints);

            await _repo.AddAsync(offer);

            await _repoOfferDetail.AddAsync(newOfferDetail);

            await _repo.SaveChangesAsync();


            var offerResponse = _mapper.Map<GetOfferDTO>(offer);


            return offerResponse;
        }

        public async Task<PagedResult<GetOfferDTO>> GetOfferSharedRideAsync(GetOfferForUserRequestDTO request)
        {
            var query = _repo.GetQueryable()
                .Include(o => o.SharedRideDetail)
                .Include(o => o.RoutePoints)
                .Include(o => o.Driver)   // <--- THÊM VÀO NGAY
                .Include(o => o.Vehicle)  // <--- THÊM VÀO NGAY
                .Where(o => o.Status == OfferStatus.Active && o.ServiceType == ServiceType.Shared);

            if (request.startTime.HasValue)
            {
                query = query.Where(o => o.DepartureTime >= request.startTime.Value);
            }

            // BƯỚC 3: Xử lý logic lọc lộ trình (Siêu quan trọng)
            if (request.PickupLocationId.HasValue && request.DropoffLocationId.HasValue)
            {
                // Có cả 2 điểm: Tìm chuyến xe có điểm đón, và TRONG CÙNG CHUYẾN ĐÓ, 
                // có điểm trả với thứ tự (OrderIndex) lớn hơn điểm đón.
                // GIẢ ĐỊNH: Entity OfferRoutePoint của bạn có property OrderIndex. Nếu chưa có, THÊM VÀO NGAY!
                query = query.Where(o => o.RoutePoints.Any(pickup =>
                    pickup.LocationId == request.PickupLocationId.Value &&
                    o.RoutePoints.Any(dropoff =>
                        dropoff.LocationId == request.DropoffLocationId.Value &&
                        pickup.Sequence < dropoff.Sequence)));
            }
            else if (request.PickupLocationId.HasValue)
            {
                // Chỉ có điểm đón: Lấy tất cả xe có đi qua điểm này
                query = query.Where(o => o.RoutePoints.Any(rp => rp.LocationId == request.PickupLocationId.Value));
            }
            else if (request.DropoffLocationId.HasValue)
            {
                // Chỉ có điểm đến: Tương tự như trên
                query = query.Where(o => o.RoutePoints.Any(rp => rp.LocationId == request.DropoffLocationId.Value));
            }

            // BƯỚC 4: Lấy tổng số để phục vụ Paging
            var totalRecords = await query.CountAsync();

            // BƯỚC 5: Phân trang, Mapping và trả về (Có AsNoTracking)
            var data = await query
                .OrderBy(o => o.DepartureTime)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .Select(o => _mapper.Map<GetOfferDTO>(o))
                .ToListAsync();

            return new PagedResult<GetOfferDTO>
            {
                Items = data,
                TotalCount = totalRecords,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<GetOfferDTO> CancelOfferAsync(Guid driverId, Guid offerId)
        {
            var offer = await GetOfferWithRelationsQuery()
                                .FirstOrDefaultAsync(x => x.Id == offerId);

            if (offer == null || offer.DeleteAt != null || offer.DriverId != driverId)
            {
                throw new AppException(404, "Không tìm thấy bản ghi hoặc bạn không có quyền hủy");
            }

            if (offer.Status != OfferStatus.Active)
            {
                throw new AppException(400, "Chỉ có thể hủy Offer ở trạng thái Active");
            }

            offer.Status = OfferStatus.Closed;

            var relatedBookings = await _bookingRepo.GetQueryable()
                                            .Where(b => b.OfferId == offerId && b.DeleteAt == null)
                                            .ToListAsync();

            foreach (var booking in relatedBookings)
            {
                booking.Status = BookingStatus.Cancelled;
                _bookingRepo.Update(booking);
            }

            _repo.Update(offer);
            await _repo.SaveChangesAsync();

            return _mapper.Map<GetOfferDTO>(offer);
        }
    }
}