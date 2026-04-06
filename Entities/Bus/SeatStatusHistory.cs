using AppGroupCar.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Bus
{
    public class SeatStatusHistory : BaseEntity
    {
        public Guid HistoryId { get; set; }

        [Required]
        public Guid SeatId { get; set; }

        [Required]
        public SeatStatus OldStatus { get; set; }

        [Required]
        public SeatStatus NewStatus { get; set; }

        [MaxLength(255)]
        public string? Reason { get; set; }

        [Required]
        public Guid ChangedBy { get; set; }

        public DateTime ChangedAt { get; set; }

        // Navigation

        public virtual Seat Seat { get; set; } = null!;

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
