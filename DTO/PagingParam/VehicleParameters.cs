using AppGroupCar.Entities.Market;
using System.ComponentModel;

namespace AppGroupCar.DTO.Filters
{
    public class VehicleParameters : PagingParameters
    {
        public string? DriverName { get; set; }

        public string? Brand { get; set; }

        public int? SeatCapacity { get; set; }

        public VehicleType? VehicleType { get; set; }

        public VehicleStatus? Status { get; set; }
    }
}
