using AppGroupCar.Entities.Market;

namespace AppGroupCar.DTO.RequestDTO.Review
{
    public class UpdateReviewRequestDTO
    {
        public Guid Id { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
    }
}
