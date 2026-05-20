namespace AppGroupCar.DTO.ResponseDTO.Statistic
{
    public class MonthlyRevenueDTO
    {
        /// <summary>
        /// Tháng (1-12)
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// N?m
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Doanh thu c?a tháng
        /// </summary>
        public decimal Revenue { get; set; }

        /// <summary>
        /// Chu?i ??nh d?ng tháng/n?m
        /// </summary>
        public string MonthYear { get; set; } = null!;
    }

    public class Last12MonthsRevenueDTO
    {
        /// <summary>
        /// Danh sách doanh thu c?a 12 tháng g?n nh?t
        /// </summary>
        public List<MonthlyRevenueDTO> MonthlyRevenues { get; set; } = new();

        /// <summary>
        /// T?ng doanh thu c?a 12 tháng
        /// </summary>
        public decimal TotalRevenue { get; set; }
    }
}
