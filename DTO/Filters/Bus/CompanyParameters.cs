namespace AppGroupCar.DTO.Filters.Bus
{
    public class CompanyParameters : PagingParameters
    {
        public string? CompanyName { get; set; }

        public string? Email { get; set; }
    }
}
