using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;
using System.ComponentModel.Design;

namespace AppGroupCar.Entities.Bus
{
    [Table("BusRoutes")]
    public class BusRoute : BaseEntity
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        // Navigation
        public ICollection<BusRoutePoint> RoutePoints { get; set; } = new List<BusRoutePoint>();

        public ICollection<BusShowtime> Showtimes { get; set; } = new List<BusShowtime>();
    }
}
