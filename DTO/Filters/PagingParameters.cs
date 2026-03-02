using System.ComponentModel;

namespace AppGroupCar.DTO.Filters
{
    public class PagingParameters
    {
        const int maxPageSize = 50; // giới hạn
        private int _pageSize = 10;

        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;

        [DefaultValue(10)]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
        public string? OrderBy { get; set; }

    }
}
