using AppGroupCar.DTO.RequestDTO.Statistic;

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
    }
}
