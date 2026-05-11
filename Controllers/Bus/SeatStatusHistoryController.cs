using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.RequestDTO.Bus.SeatStatusHistoryRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.SeatStatusHistoryResponse;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Bus.SeatStatusHistoryServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Bus
{
    [ApiController]
    [Route("seat-status-histories")]
    [Authorize(Roles = "STAFF_COMPANY")]
    public class SeatStatusHistoryController : ControllerBase
    {
        private readonly ISeatStatusHistoryService _service;

        public SeatStatusHistoryController(ISeatStatusHistoryService service)
        {
            _service = service;
        }

        private Guid GetUserId()
        {
            var userId = User.GetUserId();

            if (userId == null)
                throw new Exception("User not found in token");

            return userId.Value;
        }

        [HttpPatch("status")]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeSeatStatusRequest dto)
        {
            var userId = GetUserId();
            var result = await _service.ChangeStatusAsync(userId, dto);
            return Ok(new ApiResponse<IEnumerable<SeatStatusHistoryResponseDto>> { Data = result });
        }

        [HttpGet("seat/{seatId:guid}")]
        public async Task<IActionResult> GetBySeat(Guid seatId)
        {
            var result = await _service.GetBySeatAsync(seatId);
            return Ok(new ApiResponse<IEnumerable<SeatStatusHistoryResponseDto>> { Data = result });
        }

        [HttpGet("showtime/{showtimeId:guid}")]
        public async Task<IActionResult> GetByShowtime(Guid showtimeId)
        {
            var result = await _service.GetByShowtimeAsync(showtimeId);
            return Ok(new ApiResponse<IEnumerable<SeatStatusHistoryResponseDto>> { Data = result });
        }
    }
}
