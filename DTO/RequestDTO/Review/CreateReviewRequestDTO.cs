using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.RequestDTO.Review
{
    public class CreateReviewRequestDTO
    {
        public Guid ReferenceId { get; set; }
        public ServiceType ServiceType { get; set; }
        public Guid ReviewerId { get; set; }
        public Guid RevieweeId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
