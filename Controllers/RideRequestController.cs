using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Services.RideRequestServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RideRequestController : ControllerBase
    {
        private readonly IRideRequestService _service;

        public RideRequestController(IRideRequestService service)
        {
            _service = service;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateRideRequest(CreateRideRequestDTO rideRequest)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ hoặc bị lỗi ID!" });
            }

            var newRideRequest = await _service.CreateRideRequestAsync(userId, rideRequest);
            return Ok(new ApiResponse<RideRequest> { Data = newRideRequest });
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllRecords()
        {
            var data = await _service.GetAllRideRequestAsync();
            return Ok(new ApiResponse<IEnumerable<RideRequest>> { Data = data });
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _service.GetByIdAsync(id);
            return Ok(new ApiResponse<RideRequest> { Data = data });

        }
        [HttpPost("FilterByNameAsync")]
        public async Task<IActionResult> FilterByName(string name, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _service.FindByNameAsync(name, pageNumber, pageSize);
            return Ok(new ApiResponse<PagedResult<RideRequest>> { Data = data });
        }

        [HttpPost("FilterByTimeAndLocation")]
        public async Task<IActionResult> FilterByTimeAndLocation(string? fromlocation, string? toLocation, DateTime? fromTime, DateTime? toTime)
        {
            var data = await _service.FilterByTimeAndLocationAsync(fromlocation, toLocation, fromTime, toTime);
            return Ok(new ApiResponse<PagedResult<RideRequest>> { Data = data });
        }

        [HttpPatch("id")]
        public async Task<IActionResult> Update(UpdateRideRequestDTO rideRequest)
        {
            await _service.UpdateAsync(rideRequest);
            return Ok(new ApiResponse<string> { });
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteById(Guid rideRequestId)
        {
            await _service.DeleteByIdAsync(rideRequestId);
            return Ok(new ApiResponse<string> { });
        }
    }
}
