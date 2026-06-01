using AppGroupCar.Entities;

namespace AppGroupCar.DTO.ResponseDTO.Market
{
    public class GetOfferRoutePointDTO
    {
        public Guid Id { get; set; }
        public int Sequence { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = null!;
        public RouteStopType StopType { get; set; }
    }
}
