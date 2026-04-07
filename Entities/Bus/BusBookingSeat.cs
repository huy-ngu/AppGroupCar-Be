using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Bus
{
    [Table("BusBookingSeats")]
    [Index(nameof(BookingId), nameof(SeatNumber), IsUnique = true)]
    [Index(nameof(TicketCode), IsUnique = true)]
    public class BusBookingSeat : BaseEntity
    {
        // FK
        [Required]
        public Guid BookingId { get; set; }

        [Required]
        public Guid SeatId { get; set; }

        // Ghế
        [Required]
        [MaxLength(10)]
        public string SeatNumber { get; set; } = null!;

        // Vé / QR
        [Required]
        [MaxLength(100)]
        public string TicketCode { get; set; } = null!;

        // Check-in
        [Required]
        public bool IsCheckedIn { get; set; } = false;

        public DateTime? CheckedInAt { get; set; }

        [ForeignKey(nameof(SeatId))]
        public virtual Seat Seat { get; set; } = null!;

        // Navigation
        [ForeignKey(nameof(BookingId))]
        public BusBooking Booking { get; set; } = null!;
    }
}
