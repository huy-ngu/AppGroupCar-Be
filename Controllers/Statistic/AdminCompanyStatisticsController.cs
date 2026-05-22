using AppGroupCar.DTO.RequestDTO.Statistic;
using AppGroupCar.Services.Statistics.AdminCompanyStatistics;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Statistic
{
    [ApiController]
    [Route("[controller]")]
    public class AdminCompanyStatisticsController : ControllerBase
    {
        private readonly IAdminCompanyStatisticService _statisticService;

        public AdminCompanyStatisticsController(IAdminCompanyStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        /// <summary>
        /// Tính doanh thu của công ty theo khoảng thời gian
        /// </summary>
        /// <param name="request">Yêu cầu chứa CompanyId, StartDate, EndDate</param>
        /// <returns>Thông tin doanh thu của công ty</returns>
        [HttpPost("revenue")]
        public async Task<IActionResult> GetCompanyRevenue([FromBody] CompanyRevenueInDateRangeRequestDTO request)
        {
            var result = await _statisticService.GetCompanyRevenueAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Đếm số chuyến đi hoàn thành của công ty theo khoảng thời gian
        /// </summary>
        /// <param name="request">Yêu cầu chứa CompanyId, StartDate, EndDate</param>
        /// <returns>Thông tin số chuyến đi hoàn thành</returns>
        [HttpPost("completed-showtime-count")]
        public async Task<IActionResult> GetCompletedShowtimeCount([FromBody] CompanyRevenueInDateRangeRequestDTO request)
        {
            var result = await _statisticService.GetCompletedShowtimeCountAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Tính doanh thu của công ty cho 12 tháng gần nhất
        /// </summary>
        /// <param name="request">Yêu cầu chứa CompanyId</param>
        /// <returns>Doanh thu từng tháng và tổng doanh thu</returns>
        [HttpPost("monthly-revenue")]
        public async Task<IActionResult> GetMonthlyRevenue([FromBody] CompanyMonthlyRevenueRequestDTO request)
        {
            var result = await _statisticService.GetCompanyMonthlyRevenueAsync(request);
            return Ok(result);
        }
    }
}
