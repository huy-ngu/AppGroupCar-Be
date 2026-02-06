using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppGroupCar.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        public string Token { get; set; } // Chuỗi token random
        public string JwtId { get; set; } // Map với Jti của AccessToken (để check cặp)

        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; } // Token sống bao lâu (thường là 7-30 ngày)

        public bool Used { get; set; } // Đã dùng để renew chưa (Refresh Token Rotation)
        public bool Invalidated { get; set; } // Có bị admin thu hồi không

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
