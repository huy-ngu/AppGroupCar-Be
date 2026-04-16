namespace AppGroupCar.DTO.RequestDTO.Market.OfferRoutePoint
{
    public class UpdateOfferRoutePointRequestDTO
    {
        public Guid? OfferId { get; set; }
        public Guid? LocationId { get; set; }
        public bool? PickupAllowed { get; set; }
        public bool? DropoffAllowed { get; set; }
    }
}
