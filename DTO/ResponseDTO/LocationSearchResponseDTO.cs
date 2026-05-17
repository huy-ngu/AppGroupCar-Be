using AppGroupCar.Entities;

namespace AppGroupCar.DTO.ResponseDTO
{
    public class LocationSearchResultDTO
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public LocationType LocationType { get; set; }
        public string LocationTypeName { get; set; } = string.Empty;
        public string LocationTypeLabel { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}
