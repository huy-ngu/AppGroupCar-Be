using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.Filters
{
    public class RideRequestParameters : PagingParameters
    {
        public Guid? FromLocationId { get; set; }
        public Guid? ToLocationId { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
    }
}
