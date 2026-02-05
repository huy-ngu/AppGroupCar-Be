using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Bus
{
    [Table("BusBookings")]
    public class BusBooking
    {
        [Key]
        public Guid BookingId { get; set; } = Guid.NewGuid();

        // FK
        [Required]
        public Guid ShowtimeId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        // Giá
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // Trạng thái
        [Required]
        public BusBookingStatus Status { get; set; }

        // Audit
        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        [ForeignKey(nameof(ShowtimeId))]
        public BusShowtime Showtime { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public ICollection<BusBookingSeat> Seats { get; set; } = new List<BusBookingSeat>();
    }

    public enum BusBookingStatus
    {
        Pending = 1,
        Paid = 2,
        Cancelled = 3,
        Expired = 4
    }

}
