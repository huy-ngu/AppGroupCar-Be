using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.RequestDTO.Bus.SeatStatusHistoryRequest;
using AppGroupCar.DTO.ResponseDTO.Bus.SeatStatusHistoryResponse;
using AppGroupCar.DTO.ResponseDTO.Bus.ShowtimeResponse;
using AppGroupCar.DTO.ResponseDTO.Bus.CheckInResponse;
using AppGroupCar.Exceptions;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Bus.SeatStatusHistoryServices;
using AppGroupCar.Services.Bus.ShowtimeServices;
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
        private readonly IBusShowtimeService _showtimeService;

        public SeatStatusHistoryController(
            ISeatStatusHistoryService service,
            IBusShowtimeService showtimeService)
        {
            _service = service;
            _showtimeService = showtimeService;
        }

        private Guid GetUserId()
        {
            var userId = User.GetUserId();

            if (userId == null)
                throw new AppException(401, "User not found in token");

            return userId.Value;
        }

        private Guid GetCompanyId()
        {
            var companyId = User.GetCompanyId();

            if (companyId == null)
                throw new AppException(401, "Company not found in token");

            return companyId.Value;
        }

        [HttpGet("showtimes/upcoming")]
        public async Task<IActionResult> GetUpcomingShowtimes([FromQuery] DateOnly? fromDate)
        {
            var companyId = GetCompanyId();
            var startDate = fromDate ?? DateOnly.FromDateTime(DateTime.Today);
            var items = await _showtimeService.GetUpcomingForCompanyAsync(companyId, startDate);

            var response = new UpcomingShowtimeListResponseDto
            {
                CompanyId = companyId,
                FromDate = startDate,
                Total = items.Count,
                Items = items
            };

            return Ok(new ApiResponse<UpcomingShowtimeListResponseDto> { Data = response });
        }

        [HttpPatch("status")]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeSeatStatusRequest dto)
        {
            var companyId = GetCompanyId();
            var userId = GetUserId();
            var result = await _service.ChangeStatusAsync(companyId, userId, dto);
            return Ok(new ApiResponse<IEnumerable<SeatStatusHistoryResponseDto>> { Data = result });
        }

        [HttpGet("check-in-info")]
        public async Task<IActionResult> GetCheckInInfo(
            [FromQuery] Guid showtimeId,
            [FromQuery] string seatNumber)
        {
            var companyId = GetCompanyId();
            var result = await _service.GetCheckInInfoAsync(companyId, showtimeId, seatNumber);
            return Ok(new ApiResponse<SeatCheckInInfoResponseDto> { Data = result });
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
