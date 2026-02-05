using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Bus
{
    [Table("BusBookingSeats")]
    [Index(nameof(BookingId), nameof(SeatNumber), IsUnique = true)]
    [Index(nameof(TicketCode), IsUnique = true)]
    public class BusBookingSeat
    {
        [Key]
        public Guid BookingSeatId { get; set; } = Guid.NewGuid();

        // FK
        [Required]
        public Guid BookingId { get; set; }

        // Ghế
        [Required]
        public int SeatNumber { get; set; }

        // Vé / QR
        [Required]
        [MaxLength(100)]
        public string TicketCode { get; set; } = null!;

        // Check-in
        [Required]
        public bool IsCheckedIn { get; set; } = false;

        public DateTime? CheckedInAt { get; set; }

        // Navigation
        [ForeignKey(nameof(BookingId))]
        public BusBooking Booking { get; set; } = null!;
    }
}
