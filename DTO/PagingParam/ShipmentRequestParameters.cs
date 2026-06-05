namespace AppGroupCar.DTO.Filters
{
    public class ShipmentRequestParameters : PagingParameters
    {
        public Guid? PickupLocationId { get; set; }
        public Guid? DropoffLocationId { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
    }
}
