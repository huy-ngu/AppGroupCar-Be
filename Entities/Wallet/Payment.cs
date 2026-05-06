using AppGroupCar.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.Entities.Wallet
{
    [Table("Payments")]
    public class Payment : BaseEntity
    {
        [Required]
        public Guid BookingId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentRecordStatus Status { get; set; } = PaymentRecordStatus.Pending;
    }

    public enum PaymentMethod
    {
        Wallet = 1,
        VietQr = 2,
        WalletAndVietQr = 3
    }

    public enum PaymentRecordStatus
    {
        Pending = 1,
        Succeeded = 2,
        Failed = 3,
        Cancelled = 4,
        Refunded = 5
    }
}
