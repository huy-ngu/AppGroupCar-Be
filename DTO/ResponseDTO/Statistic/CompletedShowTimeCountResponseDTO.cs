namespace AppGroupCar.DTO.ResponseDTO.Statistic
{
    public class CompletedShowTimeCountResponseDTO
    {
        /// <summary>
        /// ID c?a công ty
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// S? l??ng chuy?n ?i hoàn thành (Status = complete)
        /// </summary>
        public int CompletedShowtimeCount { get; set; }

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
