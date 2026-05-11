using AppGroupCar.Entities;

namespace AppGroupCar.DTO.ResponseDTO
{
    public class LocationBriefResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public LocationType LocationType { get; set; }
        public string LocationTypeName { get; set; } = string.Empty;
        public Guid? ParentLocationId { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class LocationSearchResultDTO
    {
        public LocationBriefResponseDTO Location { get; set; } = new();
        public List<LocationBriefResponseDTO> ParentLocations { get; set; } = new();
        public List<LocationBriefResponseDTO> Hierarchy { get; set; } = new();
        public string DisplayName { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
