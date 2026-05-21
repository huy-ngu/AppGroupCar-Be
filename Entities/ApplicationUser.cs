using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AppGroupCar.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;

        public string? AvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }

        public decimal RatingAverage { get; set; } = 5.00m;

        public int RatingCount { get; set; } = 0;

    }
}
