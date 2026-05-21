using AppGroupCar.DTO.RequestDTO.Statistic;
using AppGroupCar.DTO.ResponseDTO.Statistic;
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

        /// <summary>
        /// Thống kê doanh thu của 12 tháng gần nhất
        /// </summary>
        /// <param name="request">Chứa ngày hiện tại</param>
        /// <returns>Danh sách doanh thu theo tháng và tổng doanh thu</returns>
        public async Task<Last12MonthsRevenueDTO> GetLast12MonthsRevenueAsync(LastMonthsRevenueRequestDTO request)
        {
            var currentDate = request.CurrentDate;
            var monthlyRevenues = new List<MonthlyRevenueDTO>();

            // Tính doanh thu cho 12 tháng gần nhất (từ tháng hiện tại quay lại 11 tháng trước)
            for (int i = 11; i >= 0; i--)
            {
                var monthDate = currentDate.AddMonths(-i);
                var startDate = new DateTime(monthDate.Year, monthDate.Month, 1);
                var endDate = startDate.AddMonths(1);

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

                var monthlyRevenue = bookingItemsRevenue + shipmentRevenue;

                monthlyRevenues.Add(new MonthlyRevenueDTO
                {
                    Month = monthDate.Month,
                    Year = monthDate.Year,
                    Revenue = monthlyRevenue,
                    MonthYear = monthDate.ToString("MM/yyyy")
                });
            }

            var result = new Last12MonthsRevenueDTO
            {
                MonthlyRevenues = monthlyRevenues,
                TotalRevenue = monthlyRevenues.Sum(m => m.Revenue)
            };

            return result;
        }

        /// <summary>
        /// Thống kê doanh thu 4 tuần của một tháng
        /// </summary>
        /// <param name="request">Chứa tháng và năm</param>
        /// <returns>Danh sách doanh thu theo tuần và tổng doanh thu của tháng</returns>
        public async Task<MonthlyWeeksRevenueDTO> GetMonthlyWeeksRevenueAsync(MonthlyWeeksRevenueRequestDTO request)
        {
            var month = request.Month;
            var year = request.Year;
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);
            var weeklyRevenues = new List<WeeklyRevenueDTO>();

            // Tính doanh thu cho 4 tuần trong tháng
            // Tuần 1: Từ ngày 1 đến ngày 7
            // Tuần 2: Từ ngày 8 đến ngày 14
            // Tuần 3: Từ ngày 15 đến ngày 21
            // Tuần 4: Từ ngày 22 đến cuối tháng
            for (int week = 1; week <= 4; week++)
            {
                DateTime weekStart;
                DateTime weekEnd;

                if (week == 1)
                {
                    weekStart = monthStart;
                    weekEnd = monthStart.AddDays(7);
                }
                else if (week == 2)
                {
                    weekStart = monthStart.AddDays(7);
                    weekEnd = monthStart.AddDays(14);
                }
                else if (week == 3)
                {
                    weekStart = monthStart.AddDays(14);
                    weekEnd = monthStart.AddDays(21);
                }
                else
                {
                    weekStart = monthStart.AddDays(21);
                    weekEnd = monthEnd;
                }

                // Tính tổng doanh thu từ booking items (chỉ các booking đã hoàn thành)
                var bookingItemsRevenue = await _bookingItemRepo.GetQueryable()
                    .Where(bi => bi.Booking.Status == BookingStatus.Compelete
                        && bi.Booking.CreatedAt >= weekStart
                        && bi.Booking.CreatedAt < weekEnd
                        && bi.Booking.DeleteAt == null
                        && bi.DeleteAt == null)
                    .SumAsync(bi => bi.Price);

                // Tính tổng doanh thu từ shipment (chỉ các shipment đã giao)
                var shipmentRevenue = await _shipmentRepo.GetQueryable()
                    .Where(s => s.ShipmentStatus == ShipmentStatus.Delivered
                        && s.CreatedAt >= weekStart
                        && s.CreatedAt < weekEnd
                        && s.DeleteAt == null)
                    .SumAsync(s => s.Price);

                var weeklyRevenue = bookingItemsRevenue + shipmentRevenue;

                weeklyRevenues.Add(new WeeklyRevenueDTO
                {
                    WeekNumber = week,
                    StartDate = weekStart,
                    EndDate = weekEnd.AddDays(-1), // Trừ 1 ngày để hiển thị ngày cuối cùng của tuần
                    Revenue = weeklyRevenue,
                    WeekRange = $"{weekStart:dd/MM/yyyy} - {weekEnd.AddDays(-1):dd/MM/yyyy}"
                });
            }

            var result = new MonthlyWeeksRevenueDTO
            {
                Month = month,
                Year = year,
                WeeklyRevenues = weeklyRevenues,
                TotalRevenue = weeklyRevenues.Sum(w => w.Revenue)
            };

            return result;
        }
    }
}

