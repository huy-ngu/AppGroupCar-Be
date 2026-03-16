using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.Filters
{
    public class BookingParameters : PagingParameters
    {
        public BookingStatus? Status { get; set; }
    }
}
