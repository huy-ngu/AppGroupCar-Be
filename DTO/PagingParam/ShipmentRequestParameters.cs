namespace AppGroupCar.DTO.Filters
{
    public class ShipmentRequestParameters : PagingParameters
    {
        public string? pickUpLocation { get; set; }
        public string? dropOffLocation { get; set; }

    }
}
