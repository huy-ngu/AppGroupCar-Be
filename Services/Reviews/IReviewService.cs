using AppGroupCar.DTO.RequestDTO.Review;
using AppGroupCar.DTO.ResponseDTO.Review;

namespace AppGroupCar.Services.Reviews
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewResponseDTO>> GetAllAsync();
        Task<IEnumerable<ReviewResponseDTO>> GetByReviewerAsync(Guid reviewerId);
        Task<IEnumerable<ReviewResponseDTO>> GetByRevieweeAsync(Guid revieweeId);
        Task<ReviewResponseDTO> GetByIdAsync(Guid reviewId);
        Task<ReviewResponseDTO> CreateAsync(CreateReviewRequestDTO request);
        Task UpdateAsync(UpdateReviewRequestDTO request);
        Task DeleteAsync(Guid reviewId);
    }
}
