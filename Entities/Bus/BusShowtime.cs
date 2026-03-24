using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Entities.Bus
{
    [Table("BusShowtimes")]
    [Index(nameof(CompanyVehicleId), nameof(DepartureDate), nameof(DepartureTime), IsUnique = true)]
    public class BusShowtime : BaseEntity
    {
        // FK
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid CompanyVehicleId { get; set; }

        [Required]
        public Guid RouteId { get; set; }

        // Thời gian
        [Required]
        [Column(TypeName = "date")]
        public DateOnly DepartureDate { get; set; }

        [Required]
        [Column(TypeName = "time")]
        public TimeOnly DepartureTime { get; set; }

        // Giá
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Trạng thái
        [Required]
        public BusShowtimeStatus Status { get; set; }

        // Audit
        [Required]
        public Guid CreatedBy { get; set; }

        //[ForeignKey(nameof(Id))]
        //public Driver Driver { get; set; } = null;

        // Navigation
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [ForeignKey(nameof(CompanyVehicleId))]
        public CompanyVehicle CompanyVehicle { get; set; } = null!;

        [ForeignKey(nameof(RouteId))]
        public BusRoute Route { get; set; } = null!;

        public ICollection<ShowtimeLog> Logs { get; set; } = new List<ShowtimeLog>();

        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }

    public enum BusShowtimeStatus
    {
        Scheduled = 1,
        Active = 2,
        Delayed = 3,
        Cancelled = 4,
        Hidden = 5
    }

}
