namespace AppGroupCar.Entities.Bus
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ShowtimeLogs")]
    public class ShowtimeLog
    {
        [Key]
        public Guid LogId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ShowtimeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = null!;

        public BusShowtimeStatus? OldStatus { get; set; }

        public BusShowtimeStatus? NewStatus { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        // Navigation
        [ForeignKey(nameof(ShowtimeId))]
        public BusShowtime Showtime { get; set; } = null!;
    }

}
