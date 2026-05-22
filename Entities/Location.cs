using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities
{
    [Table("Locations")]
    public class Location : BaseEntity
    {

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(50)]
        public string? Code { get; set; } // mã hành chính (nếu có)

        [Required]
        public LocationType LocationType { get; set; }

        public Guid? ParentLocationId { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Tọa độ không gian (SQL Server geography)
        /// </summary>
        public NetTopologySuite.Geometries.Point? Coordinate { get; set; }

        [MaxLength(100)]
        public string? GooglePlaceId { get; set; }

        // Navigation
        [ForeignKey(nameof(ParentLocationId))]
        public Location? ParentLocation { get; set; }

        public ICollection<Location> ChildLocations { get; set; } = new List<Location>();

        public ICollection<BusRoutePoint> BusRoutePoints { get; set; } = new List<BusRoutePoint>();
    }

    public enum LocationType
    {
        Province = 1,
        District = 2,
        Ward = 3,
        BusStation = 4,
        Landmark = 5,
        Depot = 6,
        Other = 99
    }

}
