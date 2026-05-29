namespace AppGroupCar.DTO.Filters
{
    public class ShipmentRequestParameters : PagingParameters
    {
        public string? pickUpLocation { get; set; }
        public string? dropOffLocation { get; set; }
        public DateTime? FromDeliveryDate { get; set; }
        public DateTime? ToDeliveryDate { get; set; }
    }
}
