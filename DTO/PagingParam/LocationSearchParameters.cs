using AppGroupCar.Entities;
using System.ComponentModel;

namespace AppGroupCar.DTO.Filters
{
    public class LocationSearchParameters
    {
        private const int MaxPageSize = 30;
        private int _pageNumber = 1;
        private int _pageSize = 10;

        public string? Keyword { get; set; }
        public LocationType? Type { get; set; }
        public Guid? ParentLocationId { get; set; }
        public bool? IsActive { get; set; } = true;

        [DefaultValue(1)]
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        [DefaultValue(10)]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 10 : Math.Min(value, MaxPageSize);
        }
    }
}
