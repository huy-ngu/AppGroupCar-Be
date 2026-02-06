using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AppGroupCar.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        //Đánh giá chung cho mọi user
        public decimal CustomerRatingAvg { get; set; } = 5.00m;

        public int CustomerRatingCount { get; set; } = 0;

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal RatingAverage { get; set; }

        public int RatingCount { get; set; }

    }
}
