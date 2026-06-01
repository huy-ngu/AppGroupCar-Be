using AppGroupCar.DTO.RequestDTO.Market.Shipment;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market.ShipmentDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Market.ShipmentOfferServices;
using Hangfire.MemoryStorage.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Market.ShipmentControllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShipmentOfferController(IShipmentOfferService _shipmentOfferService) : ControllerBase
    {
        [HttpPost("request")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> CreateFromRequest(CreateShipmentOfferRequestDTO request)
        {
            var userIdNullable = User.GetUserId();

            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var data = await _shipmentOfferService.CreateAsync(userId, request);

            return Ok(new ApiResponse<GetShipmentOfferDTO> { Code = 201, Data = data });
        }
        [HttpPost("offer")]
        [Authorize]
        public async Task<IActionResult> CreateFromOffer([FromBody] Guid offerId, Guid requestId, decimal proposedPrice)
        {
            var userIdNullable = User.GetUserId();

            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var data = await _shipmentOfferService.CreateFromOfferAsync(offerId, requestId, proposedPrice);
            return Ok(new ApiResponse<GetShipmentOfferDTO> { Code = 201, Data = data });

        }

        [HttpPost("{id}/accept")]
        [Authorize]
        public async Task<IActionResult> CustomerAcceptShipmentOffer(Guid id)
        {
            var userIdNullable = User.GetUserId();

            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            await _shipmentOfferService.AcceptAsync(id, userId);
            return Ok(new ApiResponse<string> { Code = 200 });

        }

        [HttpPost("{id}/driver-accept")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> DriverAcceptShipmentOffer(Guid id, Guid offerId)
        {
            var userIdNullable = User.GetUserId();

            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            await _shipmentOfferService.DriverAcceptAsync(userId, id, offerId);
            return Ok(new ApiResponse<string> { Code = 200 });

        }

        [HttpGet("me")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> GetMyShipmentOffers()
        {
            var userIdNullable = User.GetUserId();

            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var data = await _shipmentOfferService.GetMyShipmentOffersAsync(userId);
            return Ok(new ApiResponse<IEnumerable<GetDetailShipmentOfferDTO>> { Code = 200, Data = data });
        }

        [HttpGet()]
        public async Task<IActionResult> GetByShipmentRequest()
        {
            var data = await _shipmentOfferService.GetAll();
            return Ok(new ApiResponse<IEnumerable<ShipmentOffer>> { Code = 200, Data = data });
        }

        [HttpGet("shipmentRequestId")]
        [Authorize]
        public async Task<IActionResult> GetByShipmentRequest(Guid shipmentRequestId)
        {
            var data = await _shipmentOfferService.GetByRequestId(shipmentRequestId);
            return Ok(new ApiResponse<IEnumerable<GetDetailShipmentOfferDTO>> { Code = 200, Data = data });
        }

        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> CancelShipmentOffer(Guid id)
        {
            var userIdNullable = User.GetUserId();

            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            await _shipmentOfferService.CancelAsync(userId, id);
            return Ok(new ApiResponse<string> { Code = 200 });
        }

    }
}
