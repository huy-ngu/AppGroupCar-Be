using AppGroupCar.DTO.RequestDTO.Review;
using AppGroupCar.DTO.ResponseDTO.Review;
using AppGroupCar.Entities;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Entities.Market;
using AppGroupCar.Exceptions;
using AppGroupCar.Repositories.BaseRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppGroupCar.Services.Reviews
{
    public class ReviewService(
        IBaseRepository<Review> _reviewRepository,
        IBaseRepository<Booking> _bookingRepository,
        IBaseRepository<Shipment> _shipmentRepository,
        IBaseRepository<BusBooking> _busBookingRepository,
        IBaseRepository<ShipmentOffer> _shipmentOfferRepository,
        IBaseRepository<BusShowtime> _busShowtimeRepository,
        IBaseRepository<Driver> _driverRepository,
        UserManager<ApplicationUser> _userManager

        ) : IReviewService
    {

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

            if (request.ReferenceId == Guid.Empty)
                throw new AppException(400, "Reference ID không hợp lệ");

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

            // Validate based on ServiceType and get isReviewDriver flag
            bool isReviewDriver = false;
            if (request.ServiceType == ServiceType.Shared)
            {
                isReviewDriver = await ValidateSharedRideReviewAsync(request);
            }
            else if (request.ServiceType == ServiceType.Truck)
            {
                isReviewDriver = await ValidateShipmentReviewAsync(request);
            }
            //else if (request.ServiceType == ServiceType.Bus)
            //{
            //    isReviewDriver = await ValidateBusReviewAsync(request);
            //}
            else if (request.ServiceType == ServiceType.Contract)
            {
                isReviewDriver = await ValidateSharedRideReviewAsync(request);
            }
            else
            {
                throw new AppException(400, "ServiceType không hợp lệ");
            }

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

            // Update rating for reviewee based on whether reviewer is driver
            if (isReviewDriver)
            {
                // Update customer rating in ApplicationUser table
                await UpdateCustomerRatingAsync(request.RevieweeId, request.Rating);
            }
            else
            {
                // Update driver rating in Driver table
                UpdateDriverRating(request.RevieweeId, request.Rating);
                await _driverRepository.SaveChangesAsync();
            }

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

        /// <summary>
        /// Updates customer rating in ApplicationUser table
        /// Calculates average rating and increments rating count
        /// Uses UserManager to properly update ApplicationUser entity
        /// </summary>
        private async Task UpdateCustomerRatingAsync(Guid customerId, int newRating)
        {
            var user = await _userManager.FindByIdAsync(customerId.ToString());

            if (user == null)
                throw new AppException(404, "Khách hàng không tồn tại");

            // Calculate new average rating
            decimal currentTotal = user.RatingAverage * user.RatingCount;
            int newCount = user.RatingCount + 1;
            decimal newAverage = (currentTotal + newRating) / newCount;

            user.RatingAverage = newAverage;
            user.RatingCount = newCount;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new AppException(500, "Lỗi cập nhật rating khách hàng");
        }

        /// <summary>
        /// Updates driver rating in Driver table
        /// Calculates average rating and increments rating count
        /// </summary>
        private void UpdateDriverRating(Guid driverId, int newRating)
        {
            var driver = _driverRepository.GetQueryable()
                .Where(d => d.Id == driverId && d.DeleteAt == null)
                .FirstOrDefault();

            if (driver == null)
                throw new AppException(404, "Tài xế không tồn tại");

            // Calculate new average rating
            decimal currentTotal = driver.DriverRatingAverage * driver.DriverRatingCount;
            int newCount = driver.DriverRatingCount + 1;
            decimal newAverage = (currentTotal + newRating) / newCount;

            driver.DriverRatingAverage = newAverage;
            driver.DriverRatingCount = newCount;

            _driverRepository.Update(driver);
        }

        /// <summary>
        /// Validates shared ride booking review
        /// Ensures ReviewerId and RevieweeId are from the same booking transaction
        /// Returns true if reviewer is the driver, false if reviewer is the customer
        /// </summary>
        private async Task<bool> ValidateSharedRideReviewAsync(CreateReviewRequestDTO request)
        {
            // Get booking by ReferenceId
            var booking = await _bookingRepository.GetQueryable()
                .Where(b => b.Id == request.ReferenceId && b.DeleteAt == null)
                .Include(b => b.Offer)
                .FirstOrDefaultAsync();

            if (booking == null)
                throw new AppException(404, "Booking không tồn tại");

            // Check if booking is completed
            if (booking.Status != BookingStatus.Confirmed)
                throw new AppException(400, "Chỉ có thể đánh giá khi chuyến đã hoàn thành");

            // Get driver ID from offer
            var driverId = booking.Offer.DriverId;
            var customerId = booking.CustomerId;

            // Validate ReviewerId and RevieweeId exist in the booking
            // Reviewer must be either customer or driver
            var isValidReviewerCustomer = request.ReviewerId == customerId;
            var isValidReviewerDriver = request.ReviewerId == driverId;

            if (!isValidReviewerCustomer && !isValidReviewerDriver)
                throw new AppException(400, "Reviewer không phải là bên trong giao dịch này");

            // Reviewee must be the opposite party
            var isValidRevieweeCustomer = request.RevieweeId == customerId;
            var isValidRevieweeDriver = request.RevieweeId == driverId;

            if (!isValidRevieweeCustomer && !isValidRevieweeDriver)
                throw new AppException(400, "Reviewee không phải là bên trong giao dịch này");

            // Ensure reviewer and reviewee are different parties
            if ((isValidReviewerCustomer && isValidRevieweeCustomer) ||
                (isValidReviewerDriver && isValidRevieweeDriver))
                throw new AppException(400, "Reviewer và Reviewee phải là hai bên khác nhau trong giao dịch");

            // Return true if reviewer is driver, false if reviewer is customer
            return isValidReviewerDriver;
        }

        /// <summary>
        /// Validates shipment review
        /// Ensures ReviewerId and RevieweeId are from the same shipment transaction
        /// Returns true if reviewer is the driver, false if reviewer is the customer
        /// </summary>
        private async Task<bool> ValidateShipmentReviewAsync(CreateReviewRequestDTO request)
        {
            // Get shipment offer by ReferenceId
            var shipmentOffer = await _shipmentOfferRepository.GetQueryable()
                .Where(so => so.Id == request.ReferenceId && so.DeleteAt == null)
                .Include(so => so.Request)
                .FirstOrDefaultAsync();

            if (shipmentOffer == null)
                throw new AppException(404, "Shipment Offer không tồn tại");

            // Check if shipment is completed
            var shipment = await _shipmentRepository.GetQueryable()
                .Where(s => s.ShipmentOfferId == request.ReferenceId && s.DeleteAt == null)
                .FirstOrDefaultAsync();

            if (shipment == null || shipment.ShipmentStatus != ShipmentStatus.Delivered)
                throw new AppException(400, "Chỉ có thể đánh giá khi vận chuyển đã hoàn thành");

            // Get customer ID from shipment request and driver ID from shipment offer
            var customerId = shipmentOffer.Request.CustomerId;
            var driverId = shipmentOffer.DriverId;

            // Validate ReviewerId and RevieweeId exist in the shipment
            var isValidReviewerCustomer = request.ReviewerId == customerId;
            var isValidReviewerDriver = request.ReviewerId == driverId;

            if (!isValidReviewerCustomer && !isValidReviewerDriver)
                throw new AppException(400, "Reviewer không phải là bên trong giao dịch này");

            var isValidRevieweeCustomer = request.RevieweeId == customerId;
            var isValidRevieweeDriver = request.RevieweeId == driverId;

            if (!isValidRevieweeCustomer && !isValidRevieweeDriver)
                throw new AppException(400, "Reviewee không phải là bên trong giao dịch này");

            // Ensure reviewer and reviewee are different parties
            if ((isValidReviewerCustomer && isValidRevieweeCustomer) ||
                (isValidReviewerDriver && isValidRevieweeDriver))
                throw new AppException(400, "Reviewer và Reviewee phải là hai bên khác nhau trong giao dịch");

            // Return true if reviewer is driver, false if reviewer is customer
            return isValidReviewerDriver;
        }

        /// <summary>
        /// Validates bus booking review
        /// Ensures ReviewerId is the customer and RevieweeId is related to the bus company/route
        /// Returns false since for bus services, reviewer is always the customer (not the driver)
        /// </summary>
        private async Task<bool> ValidateBusReviewAsync(CreateReviewRequestDTO request)
        {
            // Get bus booking by ReferenceId
            var busBooking = await _busBookingRepository.GetQueryable()
                .Where(bb => bb.Id == request.ReferenceId && bb.DeleteAt == null)
                .Include(bb => bb.Showtime)
                .FirstOrDefaultAsync();

            if (busBooking == null)
                throw new AppException(404, "Bus Booking không tồn tại");

            // Check if booking is completed
            if (busBooking.Status != BusBookingStatus.Paid)
                throw new AppException(400, "Chỉ có thể đánh giá khi vé đã thanh toán và chuyến đã hoàn thành");

            // Get user ID from booking and company ID from showtime
            var userId = busBooking.UserId;
            var companyId = busBooking.Showtime.CompanyId;

            // Validate ReviewerId and RevieweeId exist in the bus booking
            var isValidReviewerUser = request.ReviewerId == userId;

            if (!isValidReviewerUser)
                throw new AppException(400, "Reviewer phải là khách hàng đã đặt vé");

            // Reviewee should be related to the company (for bus services, we accept company ID as reviewee)
            var isValidRevieweeCompany = request.RevieweeId == companyId;

            if (!isValidRevieweeCompany)
                throw new AppException(400, "Reviewee phải là công ty vận chuyển");

            // Additional check: ReviewerId and RevieweeId should be different
            if (request.ReviewerId == request.RevieweeId)
                throw new AppException(400, "Reviewer và Reviewee phải khác nhau");

            // Return false because for bus services, reviewer is always the customer (not driver)
            return false;
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
