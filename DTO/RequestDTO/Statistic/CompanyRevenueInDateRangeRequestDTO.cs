namespace AppGroupCar.DTO.RequestDTO.Statistic
{
    public class CompanyRevenueInDateRangeRequestDTO
    {
        /// <summary>
        /// ID c?a công ty
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Ngày b?t ??u c?a kho?ng th?i gian
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày k?t thúc c?a kho?ng th?i gian
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
