using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.ResponseDTO.Review
{
    public class ReviewResponseDTO
    {
        public Guid ReviewId { get; set; }
        public Guid ReferenceId { get; set; }
        public ServiceType ServiceType { get; set; }
        public Guid ReviewerId { get; set; }
        public string? ReviewerName { get; set; }
        public Guid RevieweeId { get; set; }
        public string? RevieweeName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
