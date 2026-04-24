using AppGroupCar.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.Entities.Wallet
{
    [Table("WalletTransactions")]
    public class WalletTransaction : BaseEntity
    {
        [Required]
        public Guid WalletId { get; set; }

        [Required]
        public WalletTransactionType Type { get; set; }

        [Required]
        public WalletTransactionStatus Status { get; set; } = WalletTransactionStatus.Pending;

        [Required]
        public Guid ReferenceId { get; set; }

        [MaxLength(64)]
        public string? ReferenceType { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceBefore { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAfter { get; set; }

        [ForeignKey(nameof(WalletId))]
        public Wallet Wallet { get; set; } = null!;
    }

    public enum WalletTransactionType
    {
        Deposit = 1,
        Payment = 2,
        Receive = 3,
        Refund = 4,
        Withdraw = 5
    }

    public enum WalletTransactionStatus
    {
        Pending = 1,
        Succeeded = 2,
        Failed = 3,
        Reversed = 4
    }
}
