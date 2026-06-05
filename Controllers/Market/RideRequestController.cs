using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.RideRequest;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market.RideRequest;
using AppGroupCar.Entities.Market;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Market.RideRequestServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppGroupCar.Controllers.Market
{
    [ApiController]
    [Route("[controller]")]
    public class RideRequestController : ControllerBase
    {
        private readonly IRideRequestService _service;

        public RideRequestController(IRideRequestService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRideRequest(CreateRideRequestDTO rideRequest)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token khÃ´ng há»£p lá»‡ hoáº·c bá»‹ lá»—i ID!" });
            }

            var newRideRequest = await _service.CreateRideRequestAsync(userId, rideRequest);
            return Ok(new ApiResponse<RideRequest> { Data = newRideRequest });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _service.GetByIdAsync(id);
            return Ok(new ApiResponse<RideRequest> { Data = data });
        }

        [HttpGet]
        public async Task<IActionResult> FilterByTimeAndLocation([FromQuery] RideRequestParameters parameters)
        {
            var data = await _service.FilterByTimeAndLocationAsync(parameters);
            return Ok(new ApiResponse<PagedResult<RideRequestFilterResponseDTO>> { Data = data });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyRideRequests()
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var data = await _service.GetMyRideRequestsAsync(userId);
            return Ok(new ApiResponse<IEnumerable<RideRequestFilterResponseDTO>> { Data = data });
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
