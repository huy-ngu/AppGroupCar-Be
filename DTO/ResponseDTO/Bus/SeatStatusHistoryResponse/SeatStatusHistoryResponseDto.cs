using AppGroupCar.Entities.Bus;

namespace AppGroupCar.DTO.ResponseDTO.Bus.SeatStatusHistoryResponse
{
    public class SeatStatusHistoryResponseDto
    {
        public Guid HistoryId { get; set; }
        public Guid SeatId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public SeatStatus OldStatus { get; set; }
        public SeatStatus NewStatus { get; set; }
        public string? Reason { get; set; }
        public Guid ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
