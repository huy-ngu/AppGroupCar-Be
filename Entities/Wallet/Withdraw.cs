using AppGroupCar.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.Entities.Wallet
{
    [Table("Withdrawals")]
    public class Withdraw : BaseEntity
    {
        [Required]
        public Guid WalletId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(32)]
        public string BankAcc { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string BankName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string AccName { get; set; } = string.Empty;

        [Required]
        public WithdrawStatus Status { get; set; } = WithdrawStatus.Pending;

        [MaxLength(500)]
        public string? Note { get; set; }

        [ForeignKey(nameof(WalletId))]
        public Wallet Wallet { get; set; } = null!;
    }

    public enum WithdrawStatus
    {
        Pending = 1,
        Approved = 2,
        Processing = 3,
        Paid = 4,
        Rejected = 5,
        Cancelled = 6
    }
}
