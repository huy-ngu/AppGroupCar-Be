using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.Booking;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Market.BookingServices;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AppGroupCar.Controllers.Market
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _bookingService.GetAllAsync();
            return Ok(new ApiResponse<object> { Data = response });

        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyBookings()
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var response = await _bookingService.GetMyBookingsAsync(userId);
            return Ok(new ApiResponse<object> { Data = response });
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _bookingService.GetByIdAsync(id);
            return Ok(new ApiResponse<object> { Data = response });
        }

        [HttpPost]
        public async Task<IActionResult> CreateWithItems(CreateWithBookingIteamsRequestDTO request)
        {
            var data = await _bookingService.CreateWithBookingIteamsAsync(request);
            return Ok(new ApiResponse<BookingResponseDTO> { Code = 201, Message = "Create success", Data = data });

        }
        [HttpPatch]
        public async Task<IActionResult> Update(UpdateBookingRequestDTO request)
        {
            await _bookingService.UpdateAsync(request);
            return Ok(new ApiResponse<object> { });
        }

        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            await _bookingService.CancelAsync(userId, id);
            return Ok(new ApiResponse<string> { Code = 200, Message = "Hủy booking thành công" });
        }

        [HttpPost("{id}/accept")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> AcceptBooking(Guid id)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            await _bookingService.AcceptBookingAsync(userId, id);
            return Ok(new ApiResponse<string> { Code = 200, Message = "Chấp nhận booking thành công" });
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> RejectBooking(Guid id)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            await _bookingService.RejectBookingAsync(userId, id);
            return Ok(new ApiResponse<string> { Code = 200, Message = "Từ chối booking thành công" });
        }
    }
}
