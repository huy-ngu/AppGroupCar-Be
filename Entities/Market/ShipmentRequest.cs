using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppGroupCar.Entities.Base;

namespace AppGroupCar.Entities.Market
{
    [Table("ShipmentRequests")]
    public class ShipmentRequest : BaseEntity
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid PickupLocationId { get; set; }

        [Required]
        public Guid DropoffLocationId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Weight { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Volume { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // Flags
        [Required]
        public bool IsFragile { get; set; } = false; //hàng dễ vỡ

        [Required]
        public bool IsPerishable { get; set; } = false; //hàng dễ hỏng (thực phẩm, hoa quả...)

        [Required]
        public bool RequiresCooler { get; set; } = false; //cần thùng lạnh (thực phẩm đông lạnh, dược phẩm...)

        [Required]
        public bool AvoidSunLight { get; set; } = false; // tránh ánh nắng (hàng dễ hỏng, hóa chất...)

        [MaxLength(500)]
        public string? HandlingNote { get; set; }

        [Required]
        public ShipmentRequestStatus Status { get; set; }

        public string? ImageUrl { get; set; }

        // Navigation
        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser Customer { get; set; } = null!;

        [ForeignKey(nameof(PickupLocationId))]
        public Location PickupLocation { get; set; } = null!;

        [ForeignKey(nameof(DropoffLocationId))]
        public Location DropoffLocation { get; set; } = null!;

        public ICollection<ShipmentOffer> Offers { get; set; }
            = new List<ShipmentOffer>();
    }

    public enum ShipmentRequestStatus
    {
        Open = 1,
        Cancelled = 2,
        Expired = 3
    }

}
