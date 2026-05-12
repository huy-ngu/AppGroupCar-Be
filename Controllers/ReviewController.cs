using AppGroupCar.DTO.RequestDTO.Review;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Review;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _reviewService.GetAllAsync();
            return Ok(new ApiResponse<object> { Data = response });
        }

        [HttpGet("reviewer/{reviewerId}")]
        public async Task<IActionResult> GetByReviewer(Guid reviewerId)
        {
            var response = await _reviewService.GetByReviewerAsync(reviewerId);
            return Ok(new ApiResponse<object> { Data = response });
        }

        [HttpGet("reviewee/{revieweeId}")]
        public async Task<IActionResult> GetByReviewee(Guid revieweeId)
        {
            var response = await _reviewService.GetByRevieweeAsync(revieweeId);
            return Ok(new ApiResponse<object> { Data = response });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _reviewService.GetByIdAsync(id);
            return Ok(new ApiResponse<object> { Data = response });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateReviewRequestDTO request)
        {
            var data = await _reviewService.CreateAsync(request);
            return Ok(new ApiResponse<ReviewResponseDTO> { Code = 201, Message = "Create success", Data = data });
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> Update(UpdateReviewRequestDTO request)
        {
            await _reviewService.UpdateAsync(request);
            return Ok(new ApiResponse<object> { Message = "Update success" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _reviewService.DeleteAsync(id);
            return Ok(new ApiResponse<object> { Message = "Delete success" });
        }
    }
}
