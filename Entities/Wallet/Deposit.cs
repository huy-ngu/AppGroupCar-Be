using AppGroupCar.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.Entities.Wallet
{
    [Table("Deposits")]
    public class Deposit : BaseEntity
    {
        [Required]
        public Guid WalletId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(64)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public DepositProvider Provider { get; set; }

        [MaxLength(128)]
        public string? ProviderTransactionId { get; set; }

        [Required]
        public DepositStatus Status { get; set; } = DepositStatus.Pending;

        public DateTime? ExpiredAt { get; set; }

        [ForeignKey(nameof(WalletId))]
        public Wallet Wallet { get; set; } = null!;
    }

    public enum DepositProvider
    {
        VietQr = 1,
        SePay = 2
    }

    public enum DepositStatus
    {
        Pending = 1,
        Paid = 2,
        Expired = 3,
        Failed = 4,
        Cancelled = 5
    }
}
