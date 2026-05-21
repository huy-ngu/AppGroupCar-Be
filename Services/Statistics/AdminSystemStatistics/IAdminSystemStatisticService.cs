using AppGroupCar.DTO.RequestDTO.Statistic;

using AppGroupCar.DTO.RequestDTO.Statistic;
using AppGroupCar.DTO.ResponseDTO.Statistic;

namespace AppGroupCar.Services.Statistics.AdminSystemStatistics
{
    public interface IAdminSystemStatisticService
    {
        /// <summary>
        /// Đếm số chuyến đi (offers) thành công trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Số lượng offers đã hoàn thành</returns>
        Task<int> CountSuccessfulOffersInDateRangeAsync(CountOfferInDateRangeRequestDTO request);

        /// <summary>
        /// Đếm số đơn hàng thành công (booking và shipment) trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Số lượng booking + shipment đã hoàn thành</returns>
        Task<int> CountSuccessfulOrdersInDateRangeAsync(CountOfferInDateRangeRequestDTO request);

        /// <summary>
        /// Tính tổng doanh thu từ booking items và shipment trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Tổng doanh thu từ các booking đã hoàn thành và shipment đã giao</returns>
        Task<decimal> CalculateTotalRevenueInDateRangeAsync(CountOfferInDateRangeRequestDTO request);

        /// <summary>
        /// Đếm số tài xế đang hoạt động
        /// </summary>
        /// <returns>Số lượng tài xế có trạng thái Active và chưa bị xóa</returns>
        Task<int> CountActiveDriversAsync();

        /// <summary>
        /// Đếm số phương tiện đang hoạt động
        /// </summary>
        /// <returns>Số lượng phương tiện có trạng thái Active và chưa bị xóa</returns>
        Task<int> CountActiveVehiclesAsync();

        /// <summary>
        /// Đếm số người dùng mới trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Số lượng người dùng mới được tạo trong khoảng thời gian</returns>
        Task<int> CountNewUsersInDateRangeAsync(CountOfferInDateRangeRequestDTO request);

        /// <summary>
        /// Đếm số tài xế mới trong khoảng thời gian
        /// </summary>
        /// <param name="request">Chứa ngày bắt đầu và ngày kết thúc</param>
        /// <returns>Số lượng tài xế mới được tạo trong khoảng thời gian</returns>
        Task<int> CountNewDriversInDateRangeAsync(CountOfferInDateRangeRequestDTO request);

        /// <summary>
        /// Đếm số người dùng đang hoạt động
        /// </summary>
        /// <returns>Số lượng người dùng chưa bị xóa (DeleteAt == null)</returns>
        Task<int> CountActiveUsersAsync();

        /// <summary>
        /// Thống kê doanh thu của 12 tháng gần nhất
        /// </summary>
        /// <param name="request">Chứa ngày hiện tại</param>
        /// <returns>Danh sách doanh thu theo tháng và tổng doanh thu</returns>
        Task<Last12MonthsRevenueDTO> GetLast12MonthsRevenueAsync(LastMonthsRevenueRequestDTO request);

        /// <summary>
        /// Thống kê doanh thu 4 tuần của một tháng
        /// </summary>
        /// <param name="request">Chứa tháng và năm</param>
        /// <returns>Danh sách doanh thu theo tuần và tổng doanh thu của tháng</returns>
        Task<MonthlyWeeksRevenueDTO> GetMonthlyWeeksRevenueAsync(MonthlyWeeksRevenueRequestDTO request);
    }
}
