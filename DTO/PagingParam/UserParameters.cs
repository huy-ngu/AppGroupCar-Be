using AppGroupCar.DTO.Filters;

namespace AppGroupCar.DTO.PagingParam
{
    public class UserParameters : PagingParameters
    {
        public bool? isActive { get; set; }
        public string? CreatedAt { get; set; }
    }
}
