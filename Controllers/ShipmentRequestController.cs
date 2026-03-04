using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Helper;
using AppGroupCar.Services.ShipmentRequestServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentRequestController : ControllerBase
    {
        private readonly IShipmentRequestServices _shipmentRequestServices;
        public ShipmentRequestController(IShipmentRequestServices shipmentRequestServices)
        {
            _shipmentRequestServices = shipmentRequestServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShipmentRequest(CreateShipmentRequestDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }
            var data = await _shipmentRequestServices.CreateAsync(userId, request);

            return Ok(new ApiResponse<ShipmentRequest> { Data = data });

        }
        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _shipmentRequestServices.GetByIdAsync(id);
            return Ok(new ApiResponse<ShipmentRequest> { Data = data });
        }
        [HttpPatch("id")]
        public async Task<IActionResult> Update(Guid id, UpdateShipmentRequestDTO request)
        {
            if (id == null) { return BadRequest(new ApiResponse<string> { Message = "Phải có id" }); }
            await _shipmentRequestServices.UpdateAsync(id, request);
            return Ok(new ApiResponse<ShipmentRequest> { });

        }
    }
}
