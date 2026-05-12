using AppGroupCar.DTO.RequestDTO.Review;
using AppGroupCar.DTO.ResponseDTO.Review;
using AppGroupCar.Entities;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly IBaseRepository<Review> _reviewRepository;
        public ReviewService(IBaseRepository<Review> reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<IEnumerable<ReviewResponseDTO>> GetAllAsync()
        {
            var reviews = await _reviewRepository.GetQueryable()
                .Where(r => r.DeleteAt == null)
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .Select(r => new ReviewResponseDTO
                {
                    ReviewId = r.Id,
                    ReferenceId = r.ReferenceId,
                    ServiceType = r.ServiceType,
                    ReviewerId = r.ReviewerId,
                    ReviewerName = r.Reviewer.FullName,
                    RevieweeId = r.RevieweeId,
                    RevieweeName = r.Reviewee.FullName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return reviews;
        }

        public async Task<IEnumerable<ReviewResponseDTO>> GetByReviewerAsync(Guid reviewerId)
        {
            if (reviewerId == Guid.Empty)
                throw new AppException(400, "Reviewer ID không hợp lệ");

            var reviews = await _reviewRepository.GetQueryable()
                .Where(r => r.ReviewerId == reviewerId && r.DeleteAt == null)
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .Select(r => new ReviewResponseDTO
                {
                    ReviewId = r.Id,
                    ReferenceId = r.ReferenceId,
                    ServiceType = r.ServiceType,
                    ReviewerId = r.ReviewerId,
                    ReviewerName = r.Reviewer.FullName,
                    RevieweeId = r.RevieweeId,
                    RevieweeName = r.Reviewee.FullName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return reviews;
        }

        public async Task<IEnumerable<ReviewResponseDTO>> GetByRevieweeAsync(Guid revieweeId)
        {
            if (revieweeId == Guid.Empty)
                throw new AppException(400, "Reviewee ID không hợp lệ");

            var reviews = await _reviewRepository.GetQueryable()
                .Where(r => r.RevieweeId == revieweeId && r.DeleteAt == null)
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .Select(r => new ReviewResponseDTO
                {
                    ReviewId = r.Id,
                    ReferenceId = r.ReferenceId,
                    ServiceType = r.ServiceType,
                    ReviewerId = r.ReviewerId,
                    ReviewerName = r.Reviewer.FullName,
                    RevieweeId = r.RevieweeId,
                    RevieweeName = r.Reviewee.FullName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return reviews;
        }

        public async Task<ReviewResponseDTO> GetByIdAsync(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
                throw new AppException(400, "Review ID không hợp lệ");

            var review = await _reviewRepository.GetQueryable()
                .Where(r => r.Id == reviewId && r.DeleteAt == null)
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .Select(r => new ReviewResponseDTO
                {
                    ReviewId = r.Id,
                    ReferenceId = r.ReferenceId,
                    ServiceType = r.ServiceType,
                    ReviewerId = r.ReviewerId,
                    ReviewerName = r.Reviewer.FullName,
                    RevieweeId = r.RevieweeId,
                    RevieweeName = r.Reviewee.FullName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (review == null)
                throw new AppException(404, "Không tìm thấy review");

            return review;
        }

        public async Task<ReviewResponseDTO> CreateAsync(CreateReviewRequestDTO request)
        {
            if (request == null)
                throw new AppException(400, "Request không hợp lệ");

            if (request.ReviewerId == Guid.Empty || request.RevieweeId == Guid.Empty)
                throw new AppException(400, "Reviewer ID hoặc Reviewee ID không hợp lệ");

            if (request.Rating < 1 || request.Rating > 5)
                throw new AppException(400, "Rating phải từ 1 đến 5");

            if (request.ReviewerId == request.RevieweeId)
                throw new AppException(400, "Không thể tự đánh giá chính mình");

            // Kiểm tra xem review đã tồn tại cho cặp người này chưa
            var existingReview = await _reviewRepository.GetQueryable()
                .Where(r => r.ReviewerId == request.ReviewerId &&
                            r.RevieweeId == request.RevieweeId &&
                            r.ReferenceId == request.ReferenceId &&
                            r.DeleteAt == null)
                .FirstOrDefaultAsync();

            if (existingReview != null)
                throw new AppException(400, "Đánh giá này đã tồn tại");

            // Check users exist by checking if we can load them with the review
            var newReview = new Review
            {
                Id = Guid.NewGuid(),
                ReferenceId = request.ReferenceId,
                ServiceType = request.ServiceType,
                ReviewerId = request.ReviewerId,
                RevieweeId = request.RevieweeId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(newReview);
            await _reviewRepository.SaveChangesAsync();

            // Reload with users
            var createdReview = await _reviewRepository.GetQueryable()
                .Where(r => r.Id == newReview.Id)
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .FirstOrDefaultAsync();

            return new ReviewResponseDTO
            {
                ReviewId = createdReview!.Id,
                ReferenceId = createdReview.ReferenceId,
                ServiceType = createdReview.ServiceType,
                ReviewerId = createdReview.ReviewerId,
                ReviewerName = createdReview.Reviewer?.FullName,
                RevieweeId = createdReview.RevieweeId,
                RevieweeName = createdReview.Reviewee?.FullName,
                Rating = createdReview.Rating,
                Comment = createdReview.Comment,
                CreatedAt = createdReview.CreatedAt
            };
        }

        public async Task UpdateAsync(UpdateReviewRequestDTO request)
        {
            if (request.Id == Guid.Empty)
                throw new AppException(400, "Review ID không hợp lệ");

            var review = await _reviewRepository.GetQueryable()
                .FirstOrDefaultAsync(r => r.Id == request.Id && r.DeleteAt == null);

            if (review == null)
                throw new AppException(404, "Không tìm thấy review");

            if (request.Rating.HasValue)
            {
                if (request.Rating < 1 || request.Rating > 5)
                    throw new AppException(400, "Rating phải từ 1 đến 5");

                review.Rating = request.Rating.Value;
            }

            if (request.Comment != null)
            {
                review.Comment = request.Comment;
            }

            _reviewRepository.Update(review);
            await _reviewRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
                throw new AppException(400, "Review ID không hợp lệ");

            var review = await _reviewRepository.GetQueryable()
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.DeleteAt == null);

            if (review == null)
                throw new AppException(404, "Không tìm thấy review");

            review.DeleteAt = DateTime.UtcNow;
            _reviewRepository.Update(review);
            await _reviewRepository.SaveChangesAsync();
        }
    }
}
