using AppGroupCar.Entities.Market;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.DTO.ResponseDTO.Market
{
    public class BookingResponseDTO
    {
        public Guid BookingId { get; set; }
        public Guid OfferId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public List<BookingItemResponseDTO> Items { get; set; }

    }
    public class BookingItemResponseDTO
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
