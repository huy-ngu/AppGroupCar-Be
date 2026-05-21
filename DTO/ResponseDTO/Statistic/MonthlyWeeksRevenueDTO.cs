namespace AppGroupCar.DTO.ResponseDTO.Statistic
{
    public class WeeklyRevenueDTO
    {
        /// <summary>
        /// S? tu?n trong tháng (1-4)
        /// </summary>
        public int WeekNumber { get; set; }

        /// <summary>
        /// Ngày b?t ??u tu?n
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày k?t thúc tu?n
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Doanh thu c?a tu?n
        /// </summary>
        public decimal Revenue { get; set; }

        /// <summary>
        /// Chu?i ??nh d?ng tu?n
        /// </summary>
        public string WeekRange { get; set; } = null!;
    }

    public class MonthlyWeeksRevenueDTO
    {
        /// <summary>
        /// Tháng
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// N?m
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Danh sách doanh thu c?a 4 tu?n trong tháng
        /// </summary>
        public List<WeeklyRevenueDTO> WeeklyRevenues { get; set; } = new();

        /// <summary>
        /// T?ng doanh thu c?a tháng
        /// </summary>
        public decimal TotalRevenue { get; set; }
    }
}
