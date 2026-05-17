using AppGroupCar.DTO;

namespace AppGroupCar.DTO.Filters
{
    public class TripSearchParameters : PagingParameters
    {
        public List<TripType>? Services { get; set; }

        // Bat/tat tim kiem mo rong theo ParentLocationId
        public bool EnableNearbySearch { get; set; } = false;

        // Truong xac dinh field nao duoc mo rong khi EnableNearbySearch = true
        public bool ExpandPickupLocation { get; set; } = true;
        public bool ExpandDropoffLocation { get; set; } = true;

        public DateTime? DepartureFrom { get; set; }

        public bool? IncludeBus { get; set; }
        public bool? IncludeSharedRide { get; set; }
        public bool? IncludeTruck { get; set; }

        public Guid? PickupLocationId { get; set; }
        public Guid? DropoffLocationId { get; set; }
        public DateOnly? DepartureDate { get; set; }
        public TimeOnly? DepartureTime { get; set; }
    }
}
