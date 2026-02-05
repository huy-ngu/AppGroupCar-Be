using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace AppGroupCar.Entities.Bus
{
    [Table("CompanyUsers")]
    public class CompanyUser
    {
        [Key]
        public Guid CompanyUserId { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleId { get; set; } = null!;

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        //[ForeignKey(nameof(RoleId))]
        //public Role Role { get; set; } = null!;
    }
}
