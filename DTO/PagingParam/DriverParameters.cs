using AppGroupCar.DTO.Filters;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.PagingParam
{
    public class DriverParameters : PagingParameters
    {
        public DriverVerificationStatus? VerificationStatus { get; set; }
        public string? LicenseClass { get; set; }
    }
}
