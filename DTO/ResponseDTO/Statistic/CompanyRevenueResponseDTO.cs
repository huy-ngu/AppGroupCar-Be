namespace AppGroupCar.DTO.ResponseDTO.Statistic
{
    public class CompanyRevenueResponseDTO
    {
        /// <summary>
        /// ID c?a công ty
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// T?ng doanh thu
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// S? l??ng booking có tr?ng thái thanh toán (Cash = 1 ho?c Paid = 2)
        /// </summary>
        public int BookingCount { get; set; }

        /// <summary>
        /// Ngày b?t ??u kho?ng th?i gian
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày k?t thúc kho?ng th?i gian
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
