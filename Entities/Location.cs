using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Bus;

namespace AppGroupCar.Entities
{
    [Table("Locations")]
    public class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LocationId { get; set; }

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

        [Required]
        public DateTime CreatedAt { get; set; }

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
        Landmark = 4,     // bến xe, ngã tư, cây xăng...
        Depot = 5,        // kho / bãi xe
        Other = 6
    }

}
