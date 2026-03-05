using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.Filters
{
    public class MarketDriverParameters : PagingParameters
    {
        public string? fullName { get; set; }
        public DriverVerificationStatus? status { get; set; }
    }
}
