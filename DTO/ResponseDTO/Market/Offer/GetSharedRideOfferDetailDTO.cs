namespace AppGroupCar.DTO.ResponseDTO.Market.Offer
{
    public class GetSharedRideOfferDetailDTO
    {
        public Guid Id { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
    }
}
