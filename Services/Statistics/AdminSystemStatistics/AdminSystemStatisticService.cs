using AppGroupCar.DTO.RequestDTO.Statistic;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Market;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Statistics.AdminSystemStatistics
{
    public class AdminSystemStatisticService : IAdminSystemStatisticService
    {
        private readonly IBaseRepository<Offer> _offerRepo;
        private readonly IBaseRepository<Booking> _bookingRepo;
        private readonly IBaseRepository<Shipment> _shipmentRepo;
        private readonly IBaseRepository<BookingItem> _bookingItemRepo;
        private readonly IBaseRepository<Driver> _driverRepo;
        private readonly IBaseRepository<Vehicle> _vehicleRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminSystemStatisticService(
            IBaseRepository<Offer> offerRepo,
            IBaseRepository<Booking> bookingRepo,
            IBaseRepository<Shipment> shipmentRepo,
            IBaseRepository<BookingItem> bookingItemRepo,
            IBaseRepository<Driver> driverRepo,
            IBaseRepository<Vehicle> vehicleRepo,
            UserManager<ApplicationUser> userManager)
        {
            _offerRepo = offerRepo;
            _bookingRepo = bookingRepo;
            _shipmentRepo = shipmentRepo;
            _bookingItemRepo = bookingItemRepo;
            _driverRepo = driverRepo;
            _vehicleRepo = vehicleRepo;
            _userManager = userManager;
        }

        /// <summary>
        /// Đếm số chuyến đi (offers) thành công trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Số lượng offers đã hoàn thành</returns>
        public async Task<int> CountSuccessfulOffersInDateRangeAsync(CountOfferInDateRangeRequestDTO request)
        {
            var count = await _offerRepo.GetQueryable()
                .Where(o => o.Status == OfferStatus.Complete
                    && o.CreatedAt >= request.StartDate
                    && o.CreatedAt < request.EndDate.AddDays(1)
                    && o.DeleteAt == null)
                .CountAsync();

            return count;
        }

        /// <summary>
        /// Đếm số đơn hàng thành công (booking và shipment) trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Số lượng booking + shipment đã hoàn thành</returns>
        public async Task<int> CountSuccessfulOrdersInDateRangeAsync(CountOfferInDateRangeRequestDTO request)
        {
            var startDate = request.StartDate;
            var endDate = request.EndDate.AddDays(1);

            // Đếm số booking thành công (Compelete)
            var successfulBookingsCount = await _bookingRepo.GetQueryable()
                .Where(b => b.Status == BookingStatus.Compelete
                    && b.CreatedAt >= startDate
                    && b.CreatedAt < endDate
                    && b.DeleteAt == null)
                .CountAsync();

            // Đếm số shipment đã giao (Delivered)
            var successfulShipmentsCount = await _shipmentRepo.GetQueryable()
                .Where(s => s.ShipmentStatus == ShipmentStatus.Delivered
                    && s.CreatedAt >= startDate
                    && s.CreatedAt < endDate
                    && s.DeleteAt == null)
                .CountAsync();

            return successfulBookingsCount + successfulShipmentsCount;
        }

        /// <summary>
        /// Tính tổng doanh thu từ booking items và shipment trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Tổng doanh thu từ các booking đã hoàn thành và shipment đã giao</returns>
        public async Task<decimal> CalculateTotalRevenueInDateRangeAsync(CountOfferInDateRangeRequestDTO request)
        {
            var startDate = request.StartDate;
            var endDate = request.EndDate.AddDays(1);

            // Tính tổng doanh thu từ booking items (chỉ các booking đã hoàn thành)
            var bookingItemsRevenue = await _bookingItemRepo.GetQueryable()
                .Where(bi => bi.Booking.Status == BookingStatus.Compelete
                    && bi.Booking.CreatedAt >= startDate
                    && bi.Booking.CreatedAt < endDate
                    && bi.Booking.DeleteAt == null
                    && bi.DeleteAt == null)
                .SumAsync(bi => bi.Price);

            // Tính tổng doanh thu từ shipment (chỉ các shipment đã giao)
            var shipmentRevenue = await _shipmentRepo.GetQueryable()
                .Where(s => s.ShipmentStatus == ShipmentStatus.Delivered
                    && s.CreatedAt >= startDate
                    && s.CreatedAt < endDate
                    && s.DeleteAt == null)
                .SumAsync(s => s.Price);

            return bookingItemsRevenue + shipmentRevenue;
        }

        /// <summary>
        /// Đếm số tài xế đang hoạt động
        /// </summary>
        /// <returns>Số lượng tài xế có trạng thái Active và chưa bị xóa</returns>
        public async Task<int> CountActiveDriversAsync()
        {
            var count = await _driverRepo.GetQueryable()
                .Where(d => d.VerificationStatus == DriverVerificationStatus.Active
                    && d.DeleteAt == null)
                .CountAsync();

            return count;
        }

        /// <summary>
        /// Đếm số phương tiện đang hoạt động
        /// </summary>
        /// <returns>Số lượng phương tiện có trạng thái Active và chưa bị xóa</returns>
        public async Task<int> CountActiveVehiclesAsync()
        {
            var count = await _vehicleRepo.GetQueryable()
                .Where(v => v.Status == VehicleStatus.Active
                    && v.DeleteAt == null)
                .CountAsync();

            return count;
        }

        /// <summary>
        /// Đếm số người dùng mới trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Số lượng người dùng mới được tạo trong khoảng thời gian</returns>
        public async Task<int> CountNewUsersInDateRangeAsync(CountOfferInDateRangeRequestDTO request)
        {
            var startDate = request.StartDate;
            var endDate = request.EndDate.AddDays(1);

            var count = await _userManager.Users
                .Where(u => u.CreatedAt >= startDate
                    && u.CreatedAt < endDate
                    && u.DeleteAt == null)
                .CountAsync();

            return count;
        }

        /// <summary>
        /// Đếm số tài xế mới trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Số lượng tài xế mới được tạo trong khoảng thời gian</returns>
        public async Task<int> CountNewDriversInDateRangeAsync(CountOfferInDateRangeRequestDTO request)
        {
            var startDate = request.StartDate;
            var endDate = request.EndDate.AddDays(1);

            var count = await _driverRepo.GetQueryable()
                .Where(d => d.CreatedAt >= startDate
                    && d.CreatedAt < endDate
                    && d.DeleteAt == null)
                .CountAsync();

            return count;
        }

        /// <summary>
        /// Đếm số người dùng đang hoạt động
        /// </summary>
        /// <returns>Số lượng người dùng chưa bị xóa (DeleteAt == null)</returns>
        public async Task<int> CountActiveUsersAsync()
        {
            var count = await _userManager.Users
                .Where(u => u.DeleteAt == null)
                .CountAsync();

            return count;
        }
    }
}
