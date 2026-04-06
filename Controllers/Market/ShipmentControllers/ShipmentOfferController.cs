using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Market.ShipmentOfferServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Market.ShipmentControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipmentOfferController(IShipmentOfferService _shipmentOfferService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> Create(CreateShipmentOfferRequestDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var data = await _shipmentOfferService.CreateAsync(userId, request);

            return Ok(new ApiResponse<GetShipmentOfferDTO> { Code = 201, Data = data });
        }
        [HttpGet()]
        public async Task<IActionResult> GetByShipmentRequest()
        {
            var data = await _shipmentOfferService.GetAll();
            return Ok(new ApiResponse<IEnumerable<ShipmentOffer>> { Code = 201, Data = data });
        }

        [HttpGet("shipmentRequestId")]
        [Authorize]
        public async Task<IActionResult> GetByShipmentRequest(Guid shipmentRequestId)
        {
            var data = await _shipmentOfferService.GetByRequestId(shipmentRequestId);
            return Ok(new ApiResponse<IEnumerable<GetShipmentOfferDTO>> { Code = 201, Data = data });
        }
    }
}
