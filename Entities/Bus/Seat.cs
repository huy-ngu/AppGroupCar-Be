using AppGroupCar.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Bus
{
    public class Seat : BaseEntity
    {
        public Guid SeatId { get; set; }

        [Required]
        public Guid ShowtimeId { get; set; }

        [Required]
        [MaxLength(10)]
        public string SeatNumber { get; set; } = null!;

        [Required]
        public SeatStatus Status { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        // Navigation

        public virtual BusShowtime Showtime { get; set; } = null!;

        public virtual ICollection<BusBookingSeat> BookingSeats { get; set; }
            = new List<BusBookingSeat>();

        public virtual ICollection<SeatStatusHistory> SeatStatusHistories { get; set; }
            = new List<SeatStatusHistory>();
    }
}

public enum SeatStatus
{
    Available = 0,
    Booked = 1,
    Disabled = 2
}
