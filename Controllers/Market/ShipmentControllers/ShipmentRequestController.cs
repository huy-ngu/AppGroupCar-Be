using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.ShipmentRequest;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Market.ShipmentRequestServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppGroupCar.Controllers.Market.ShipmentControllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShipmentRequestController : ControllerBase
    {
        private readonly IShipmentRequestServices _shipmentRequestServices;

        public ShipmentRequestController(IShipmentRequestServices shipmentRequestServices)
        {
            _shipmentRequestServices = shipmentRequestServices;
        }

        [HttpGet]
        public async Task<IActionResult> Filter([FromQuery] ShipmentRequestParameters param)
        {
            var data = await _shipmentRequestServices.FilterAsync(param);
            return Ok(new ApiResponse<PagedResult<ShipmentRequestFilterResponseDTO>> { Data = data });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyShipmentRequests()
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });            }

            var data = await _shipmentRequestServices.GetMyShipmentRequestsAsync(userId);
            return Ok(new ApiResponse<IEnumerable<ShipmentRequestFilterResponseDTO>> { Data = data });
        }

        [HttpPost]
        public async Task<IActionResult> CreateShipmentRequest([FromForm] CreateShipmentRequestDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });            }
            var data = await _shipmentRequestServices.CreateAsync(userId, request);
            return Ok(new ApiResponse<ShipmentRequest> { Data = data });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _shipmentRequestServices.GetByIdAsync(id);
            return Ok(new ApiResponse<ShipmentRequest> { Data = data });
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] UpdateShipmentRequestDTO request)
        {
            await _shipmentRequestServices.UpdateAsync(request);
            return Ok(new ApiResponse<ShipmentRequest> { });
        }
    }
}
