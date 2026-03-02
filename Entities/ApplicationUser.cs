using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AppGroupCar.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;

        //Đánh giá chung cho mọi user
        public decimal CustomerRatingAvg { get; set; } = 5.00m;

        public string? AvatarUrl { get; set; }
        public int CustomerRatingCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }

        public decimal RatingAverage { get; set; }

        public int RatingCount { get; set; }

    }
}
