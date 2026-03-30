using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Market
{
    [Table("Bookings")]
    public class Booking : BaseEntity
    {

        [Required]
        public Guid OfferId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        public BookingStatus Status { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }

        [Required]
        public int Quantity { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }


        // Navigation
        [ForeignKey(nameof(OfferId))]
        public Offer Offer { get; set; } = null!;

        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser Customer { get; set; } = null!;

        public ICollection<BookingItem> Items { get; set; }
            = new List<BookingItem>();
    }

    public enum BookingStatus
    {
        Pending = 1,
        Confirmed = 2,
        Cancelled = 3
    }

    public enum PaymentStatus
    {
        Unpaid = 1,
        Paid = 2,
        Refunded = 3
    }


}
