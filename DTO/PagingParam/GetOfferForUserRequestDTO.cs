using AppGroupCar.DTO.Filters;

namespace AppGroupCar.DTO.RequestDTO.Market.Offer
{
    public class GetOfferForUserRequestDTO : PagingParameters
    {
        public Guid? PickupLocationId { get; set; }
        public Guid? DropoffLocationId { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public decimal? startPrice { get; set; }
        public decimal? endPrice { get; set; }        
    }
}
