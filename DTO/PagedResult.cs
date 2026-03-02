namespace AppGroupCar.DTO
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Tự động tính toán tổng số trang
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        // Cờ tiện ích cho Frontend biết có thể bấm nút "Next" hoặc "Previous" không
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
