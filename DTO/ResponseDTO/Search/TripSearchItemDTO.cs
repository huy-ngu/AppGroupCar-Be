using AppGroupCar.DTO;

namespace AppGroupCar.DTO.ResponseDTO.Search
{
    public enum TripLocationMatchType
    {
        None = 0,
        Exact = 1,
        Sibling = 2,
        Parent = 3
    }

    public class TripSearchReferenceDTO
    {
        public Guid Id { get; set; }
        public TripType TripType { get; set; }
        public string TripTypeName => TripType switch
        {
            TripType.Bus => "Xe bus",
            TripType.SharedRide => "Xe ghep",
            TripType.Truck => "Xe tai",
            _ => "Khac"
        };
        public string ServiceCode { get; set; } = string.Empty;
        public string DetailApi { get; set; } = string.Empty;
    }

    public class TripSearchMatchDTO
    {
        public Guid? ExactPickupLocationId { get; set; }
        public Guid? ExactDropoffLocationId { get; set; }
        public Guid? MatchedPickupLocationId { get; set; }
        public Guid? MatchedDropoffLocationId { get; set; }
        public TripLocationMatchType PickupMatchType { get; set; }
        public TripLocationMatchType DropoffMatchType { get; set; }
        public bool UsedNearbyForPickup { get; set; }
        public bool UsedNearbyForDropoff { get; set; }
    }

    public class TripSearchItemDTO
    {
        public Guid TripId { get; set; }
        public TripType TripType { get; set; }
        public string TripTypeName => TripType switch
        {
            TripType.Bus => "Xe bus",
            TripType.SharedRide => "Xe ghep",
            TripType.Truck => "Xe tai",
            _ => "Khac"
        };
        public string ProviderName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public decimal Price { get; set; }
        public List<TripRoutePointDTO> RoutePoints { get; set; } = new();
        public TripSearchReferenceDTO Reference { get; set; } = new();
        public TripSearchMatchDTO Match { get; set; } = new();
    }

    public class TripRoutePointDTO
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public bool PickupAllowed { get; set; }
        public bool DropoffAllowed { get; set; }
    }
}
