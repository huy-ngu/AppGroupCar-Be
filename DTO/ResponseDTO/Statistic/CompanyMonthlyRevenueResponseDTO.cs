namespace AppGroupCar.DTO.ResponseDTO.Statistic
{
    public class CompanyMonthlyRevenueResponseDTO
    {
        /// <summary>
        /// ID c?a công ty
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// T?ng doanh thu 12 tháng
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Danh sách doanh thu t?ng tháng
        /// </summary>
        public List<MonthlyRevenueDetailDTO> MonthlyRevenues { get; set; } = new List<MonthlyRevenueDetailDTO>();
    }

    public class MonthlyRevenueDetailDTO
    {
        /// <summary>
        /// N?m (ví d?: 2024)
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Tháng (1-12)
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Doanh thu trong tháng
        /// </summary>
        public decimal Revenue { get; set; }

        /// <summary>
        /// S? l??ng booking trong tháng (Status = Cash ho?c Paid)
        /// </summary>
        public int BookingCount { get; set; }
    }
}
