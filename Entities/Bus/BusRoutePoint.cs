using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Bus
{
    [Table("BusRoutePoints")]
    [Index(nameof(RouteId), nameof(Sequence), IsUnique = true)]
    public class BusRoutePoint : BaseEntity
    {
        [Required]
        public Guid RouteId { get; set; }

        [Required]
        public Guid LocationId { get; set; }

        [Required]
        public int Sequence { get; set; }

        [Required]
        public RouteStopType StopType { get; set; }

        // Navigation
        [ForeignKey(nameof(RouteId))]
        public BusRoute Route { get; set; } = null!;

        [ForeignKey(nameof(LocationId))]
        public Location Location { get; set; } = null!;
    }

}
