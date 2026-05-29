using AppGroupCar.Entities.Bus;

namespace AppGroupCar.DTO.ResponseDTO.Bus.CheckInResponse
{
    public class SeatCheckInInfoResponseDto
    {
        public Guid BookingId { get; set; }
        public string TicketCode { get; set; } = null!;
        public Guid ShowtimeId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public BusBookingStatus BookingStatus { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerFullName { get; set; } = null!;
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public bool IsCheckedIn { get; set; }
        public DateTime? CheckedInAt { get; set; }
    }
}
