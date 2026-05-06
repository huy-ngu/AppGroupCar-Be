using AppGroupCar.DTO.Filters;
using AppGroupCar.Entities.Bus;

namespace AppGroupCar.DTO.PagingParam
{
    public class BusShowtimeParameters : PagingParameters
    {
        public string? PlateNumber { get; set; } = null!;
        public DateOnly? DepartureDate { get; set; }
        public BusShowtimeStatus? Status { get; set; }
    }
}
