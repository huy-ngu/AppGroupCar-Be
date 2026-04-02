using AppGroupCar.Entities;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.RequestDTO
{
    public class CreateLocationRequestDTO
    {
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(50)]
        //public string? Code { get; set; }

        [Required]
        public LocationType LocationType { get; set; }

        public Guid? ParentLocationId { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        //public NetTopologySuite.Geometries.Point? Coordinate { get; set; }

        //[MaxLength(100)]
        //public string? GooglePlaceId { get; set; }
    }
}
