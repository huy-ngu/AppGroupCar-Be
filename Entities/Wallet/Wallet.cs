using AppGroupCar.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.Entities.Wallet
{
    [Table("Wallets")]
    public class Wallet : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; private set; } = 0;

        [Required]
        public WalletStatus Status { get; set; } = WalletStatus.Active;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        public ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
        public ICollection<Withdraw> Withdrawals { get; set; } = new List<Withdraw>();
        public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();

        public void ApplyBalance(decimal balanceAfter)
        {
            if (balanceAfter < 0)
            {
                throw new InvalidOperationException("Wallet balance cannot be negative.");
            }

            Balance = balanceAfter;
            LastUpdateAt = DateTime.UtcNow;
        }
    }

    public enum WalletStatus
    {
        Active = 1,
        Locked = 2,
        Closed = 3
    }
}
