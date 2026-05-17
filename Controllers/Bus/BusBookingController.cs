using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.RequestDTO.Bus.BookingRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.BookingResponse;
using AppGroupCar.Exceptions;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Bus.BookingServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Bus
{
    [ApiController]
    [Route("bus-bookings")]
    [Authorize]
    public class BusBookingController : ControllerBase
    {
        private readonly IBusBookingService _service;

        public BusBookingController(IBusBookingService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var userId = User.GetUserId();

            if (userId == null)
                throw new AppException(401, "User not found in token");

            return userId.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBusBookingRequest dto)
        {
            var userId = GetUserId();
            var result = await _service.CreateAsync(userId, dto);
            return Ok(new ApiResponse<BusBookingResponseDto> { Code = 201, Message = "Create success", Data = result });
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = GetUserId();
            var result = await _service.GetMyBookingsAsync(userId);
            return Ok(new ApiResponse<IEnumerable<BusBookingResponseDto>> { Data = result });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetUserId();
            var result = await _service.GetByIdAsync(id, userId);
            return Ok(new ApiResponse<BusBookingResponseDto> { Data = result });
        }
    }
}
