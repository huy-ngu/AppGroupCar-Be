using AppGroupCar.DTO.Filters;
using AppGroupCar.Entities.Market;
using System.ComponentModel.DataAnnotations;

namespace AppGroupCar.DTO.PagingParam
{
    public class CompanyVehicleParameters : PagingParameters
    {
        public VehicleType? VehicleType { get; set; }
        public VehicleStatus? Status { get; set; }
    }
}
