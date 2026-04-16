namespace AppGroupCar.DTO.RequestDTO.Market.OfferRoutePoint
{
    public class CreateOfferRoutePointRequestDTO
    {
        public Guid OfferId { get; set; }
        public int Sequence { get; set; }
        public Guid LocationId { get; set; }
        public bool PickupAllowed { get; set; }
        public bool DropoffAllowed { get; set; }
    }
}
