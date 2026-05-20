using AppGroupCar.DTO.RequestDTO.Statistic;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Statistic;
using AppGroupCar.Services.Statistics.AdminSystemStatistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Statistic
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminSystemStatisticController : ControllerBase
    {
        private readonly IAdminSystemStatisticService _adminSystemStatisticService;

        public AdminSystemStatisticController(IAdminSystemStatisticService adminSystemStatisticService)
        {
            _adminSystemStatisticService = adminSystemStatisticService;
        }

        /// <summary>
        /// Đếm số chuyến đi (offers) thành công trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Số lượng offers đã hoàn thành</returns>
        [HttpPost("successful-offers")]
        public async Task<IActionResult> CountSuccessfulOffers([FromBody] CountOfferInDateRangeRequestDTO request)
        {
            var count = await _adminSystemStatisticService.CountSuccessfulOffersInDateRangeAsync(request);
            return Ok(new ApiResponse<int>
            {
                Data = count,
                Message = "Đếm số chuyến đi thành công"
            });
        }

        /// <summary>
        /// Đếm số đơn hàng thành công (booking và shipment) trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Số lượng booking + shipment đã hoàn thành</returns>
        [HttpPost("successful-orders")]
        public async Task<IActionResult> CountSuccessfulOrders([FromBody] CountOfferInDateRangeRequestDTO request)
        {
            var count = await _adminSystemStatisticService.CountSuccessfulOrdersInDateRangeAsync(request);
            return Ok(new ApiResponse<int>
            {
                Data = count,
                Message = "Đếm số đơn hàng thành công"
            });
        }

        /// <summary>
        /// Tính tổng doanh thu từ booking items và shipment trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Tổng doanh thu từ các booking đã hoàn thành và shipment đã giao</returns>
        [HttpPost("total-revenue")]
        public async Task<IActionResult> CalculateTotalRevenue([FromBody] CountOfferInDateRangeRequestDTO request)
        {
            var revenue = await _adminSystemStatisticService.CalculateTotalRevenueInDateRangeAsync(request);
            return Ok(new ApiResponse<decimal>
            {
                Data = revenue,
                Message = "Tính tổng doanh thu"
            });
        }

        /// <summary>
        /// Đếm số người dùng đang hoạt động
        /// </summary>
        /// <returns>Số lượng người dùng chưa bị xóa</returns>
        [HttpGet("active-users")]
        public async Task<IActionResult> CountActiveUsers()
        {
            var count = await _adminSystemStatisticService.CountActiveUsersAsync();
            return Ok(new ApiResponse<int>
            {
                Data = count,
                Message = "Đếm số người dùng đang hoạt động"
            });
        }

        /// <summary>
        /// Đếm số tài xế đang hoạt động
        /// </summary>
        /// <returns>Số lượng tài xế có trạng thái Active</returns>
        [HttpGet("active-drivers")]
        public async Task<IActionResult> CountActiveDrivers()
        {
            var count = await _adminSystemStatisticService.CountActiveDriversAsync();
            return Ok(new ApiResponse<int>
            {
                Data = count,
                Message = "Đếm số tài xế đang hoạt động"
            });
        }

        /// <summary>
        /// Đếm số phương tiện đang hoạt động
        /// </summary>
        /// <returns>Số lượng phương tiện có trạng thái Active</returns>
        [HttpGet("active-vehicles")]
        public async Task<IActionResult> CountActiveVehicles()
        {
            var count = await _adminSystemStatisticService.CountActiveVehiclesAsync();
            return Ok(new ApiResponse<int>
            {
                Data = count,
                Message = "Đếm số phương tiện đang hoạt động"
            });
        }

        /// <summary>
        /// Đếm số người dùng mới trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Số lượng người dùng mới được tạo</returns>
        [HttpPost("new-users")]
        public async Task<IActionResult> CountNewUsers([FromBody] CountOfferInDateRangeRequestDTO request)
        {
            var count = await _adminSystemStatisticService.CountNewUsersInDateRangeAsync(request);
            return Ok(new ApiResponse<int>
            {
                Data = count,
                Message = "Đếm số người dùng mới"
            });
        }

        /// <summary>
        /// Đếm số tài xế mới trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Số lượng tài xế mới được tạo</returns>
        [HttpPost("new-drivers")]
        public async Task<IActionResult> CountNewDrivers([FromBody] CountOfferInDateRangeRequestDTO request)
        {
            var count = await _adminSystemStatisticService.CountNewDriversInDateRangeAsync(request);
            return Ok(new ApiResponse<int>
            {
                Data = count,
                Message = "Đếm số tài xế mới"
            });
        }

        /// <summary>
        /// Thống kê doanh thu của 12 tháng gần nhất
        /// </summary>
        /// <param name="request">Chứa ngày hiện tại</param>
        /// <returns>Danh sách doanh thu theo tháng và tổng doanh thu</returns>
        [HttpPost("last-12-months-revenue")]
        public async Task<IActionResult> GetLast12MonthsRevenue([FromBody] LastMonthsRevenueRequestDTO request)
        {
            var result = await _adminSystemStatisticService.GetLast12MonthsRevenueAsync(request);
            return Ok(new ApiResponse<Last12MonthsRevenueDTO>
            {
                Data = result,
                Message = "Thống kê doanh thu 12 tháng gần nhất"
            });
        }
    }
}
