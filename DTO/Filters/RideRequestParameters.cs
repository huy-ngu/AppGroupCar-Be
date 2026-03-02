using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.Filters
{
    public class RideRequestParameters
    {
        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }

        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }

        // Có thể thêm trạng thái để lọc
        public RideRequestStatus? Status { get; set; }

        // Có thể thêm tham số sắp xếp
        public string? OrderBy { get; set; }
    }
}
