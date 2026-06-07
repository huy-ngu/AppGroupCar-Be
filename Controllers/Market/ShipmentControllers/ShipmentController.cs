using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Market.ShipmentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Market.ShipmentControllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShipmentController(IShipmentService shipmentService) : ControllerBase
    {
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var data = await shipmentService.GetDetailByIdAsync(userId, id);
            return Ok(new ApiResponse<GetShipmentDetailDTO> { Code = 200, Data = data });
        }

        [HttpGet("offer/{shipmentOfferId}")]
        [Authorize]
        public async Task<IActionResult> GetByOfferId(Guid shipmentOfferId)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var data = await shipmentService.GetDetailByOfferIdAsync(userId, shipmentOfferId);
            return Ok(new ApiResponse<GetShipmentDetailDTO> { Code = 200, Data = data });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyShipments()
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var data = await shipmentService.GetMyShipmentsAsync(userId);
            return Ok(new ApiResponse<IEnumerable<GetShipmentDTO>> { Code = 200, Data = data });
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateShipmentDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            await shipmentService.UpdateAsync(userId, request);
            return Ok(new ApiResponse<GetShipmentDTO> { Code = 200 });
        }

        // [HttpPatch("compelete")]
        // [Authorize(Roles = "Driver")]
        // public async Task<IActionResult> Compelete(Guid id)
        // {
        //     await shipmentService.Compelete(id);
        //     return Ok(new ApiResponse<GetShipmentDTO> { Code = 200 });
        // }

        // [HttpPatch("return")]
        // [Authorize(Roles = "Driver")]
        // public async Task<IActionResult> Return(Guid id)
        // {
        //     await shipmentService.ReturnDelivery(id);
        //     return Ok(new ApiResponse<GetShipmentDTO> { Code = 200 });
        // }

        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            await shipmentService.CancelAsync(userId, id);
            return Ok(new ApiResponse<string> { Code = 200, Message = "Hủy hàng thành công" });
        }

        [HttpPatch("status")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateShipmentStatusDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            await shipmentService.DriverUpdateStatusAsync(userId, request);
            return Ok(new ApiResponse<GetShipmentDTO> { Code = 200 });
        }
    }
}
