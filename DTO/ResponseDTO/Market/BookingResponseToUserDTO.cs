using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Market
{
    public class BookingResponseToUserDTO
    {
        public Guid BookingId { get; set; }
        public Guid OfferId { get; set; }
        public string DriverName { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
