using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Bus
{
    [Table("BusRoutes")]
    public class BusRoute : BaseEntity
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        // Navigation
        public ICollection<BusRoutePoint> RoutePoints { get; set; } = new List<BusRoutePoint>();

        public ICollection<BusShowtime> Showtimes { get; set; } = new List<BusShowtime>();
    }
}
