using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.Entities.Bus
{
    [Table("BusRoutes")]
    public class BusRoute
    {
        [Key]
        public Guid RouteId { get; set; } = Guid.NewGuid();

        [MaxLength(200)]
        public string? Name { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<BusRoutePoint> RoutePoints { get; set; } = new List<BusRoutePoint>();

        public ICollection<BusShowtime> Showtimes { get; set; } = new List<BusShowtime>();
    }
}
