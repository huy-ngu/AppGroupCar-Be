namespace AppGroupCar.Services.Market.BookingOfferServices
{
    public interface IBookingOfferService
    {
        Task CreateAsync(Guid RideRequestId, Guid BookingId);
        Task GetByRideRequestAsync(Guid RideRequestId);
        Task AcceptAsync(Guid BookingOfferId);
        Task RejectAsync(Guid BookingOfferId);
    }
}
