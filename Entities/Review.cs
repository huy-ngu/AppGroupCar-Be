using AppGroupCar.Entities.Base;
using AppGroupCar.Entities.Market;

namespace AppGroupCar.Entities
{
    public class Review : BaseEntity
    {
        // Liên kết đa hình (Polymorphic)
        public Guid ReferenceId { get; set; }
        public ServiceType ServiceType { get; set; }

        // Khóa ngoại: Người đánh giá
        public Guid ReviewerId { get; set; }
        public virtual ApplicationUser Reviewer { get; set; } = null!;

        // Khóa ngoại: Người bị đánh giá
        public Guid RevieweeId { get; set; }
        public virtual ApplicationUser Reviewee { get; set; } = null!;

        // Dữ liệu đánh giá
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}