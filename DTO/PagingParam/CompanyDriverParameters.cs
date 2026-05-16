using AppGroupCar.DTO.Filters;

namespace AppGroupCar.DTO.PagingParam
{
    public class CompanyDriverParameters : PagingParameters
    {
        public int? DriverVerificationStatus { get; set; }
    }
}
