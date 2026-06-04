using AppGroupCar.Entities.Bus;

namespace AppGroupCar.DTO.ResponseDTO.Bus.BookingResponse
{
    public class BusBookingResponseDto
    {
        public Guid BookingId { get; set; }
        public Guid ShowtimeId { get; set; }
        public Guid UserId { get; set; }
        public Guid PickupLocationId { get; set; }
        public Guid DropoffLocationId { get; set; }
        public decimal TotalPrice { get; set; }
        public BusBookingStatus Status { get; set; }
        public DateTime? ExpireAt { get; set; }
        public List<BusBookingSeatResponseDto> Seats { get; set; } = new();
    }

    public class BusBookingSeatResponseDto
    {
        public Guid SeatId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public string TicketCode { get; set; } = null!;
        public bool IsCheckedIn { get; set; }
        public DateTime? CheckedInAt { get; set; }
    }
}
