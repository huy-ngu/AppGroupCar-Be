using AppGroupCar.DTO.RequestDTO.Statistic;
using AppGroupCar.DTO.RequestDTO.Statistic;
using AppGroupCar.DTO.ResponseDTO.Statistic;

namespace AppGroupCar.Services.Statistics.AdminCompanyStatistics
{
    public interface IAdminCompanyStatisticService
    {
        /// <summary>
        /// Tính doanh thu của công ty theo khoảng thời gian
        /// </summary>
        /// <param name="request">Yêu cầu chứa CompanyId, StartDate, EndDate</param>
        /// <returns>Thông tin doanh thu của công ty</returns>
        Task<CompanyRevenueResponseDTO> GetCompanyRevenueAsync(CompanyRevenueInDateRangeRequestDTO request);

        /// <summary>
        /// Đếm số chuyến đi hoàn thành của công ty theo khoảng thời gian
        /// </summary>
        /// <param name="request">Yêu cầu chứa CompanyId, StartDate, EndDate</param>
        /// <returns>Thông tin số chuyến đi hoàn thành</returns>
        Task<CompletedShowTimeCountResponseDTO> GetCompletedShowtimeCountAsync(CompanyRevenueInDateRangeRequestDTO request);
    }
}
