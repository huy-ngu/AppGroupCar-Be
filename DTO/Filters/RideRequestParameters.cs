using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.Filters
{
    public class RideRequestParameters : PagingParameters
    {
        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
    }
}
