using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market.Offer;
using AppGroupCar.Entities.Market;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Market.OfferRoutePointServices;
using AppGroupCar.Services.Market.OfferServices;
using AppGroupCar.Services.Market.OfferServices.OfferShipmentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppGroupCar.Controllers.Market
{
    [ApiController]
    [Route("[controller]")]
    public class OfferController(IOfferService _offerService, IOfferShipmentSerivce _offerShipmentSerivce) : ControllerBase
    {

        [HttpGet("shared-ride")]
        public async Task<IActionResult> SearchOfferSharedRide([FromQuery] GetOfferForUserRequestDTO request)
        {
            var data = await _offerService.GetOfferSharedRideAsync(request);
            return Ok(new ApiResponse<PagedResult<GetOfferSharedRideDTO>> { Data = data });
        }
        [HttpGet("shipment")]
        public async Task<IActionResult> SearchOfferShipment([FromQuery] GetOfferForUserRequestDTO request)
        {
            var data = await _offerShipmentSerivce.GetOfferShipmentAsync(request);
            return Ok(new ApiResponse<PagedResult<GetOfferShipmentDTO>> { Data = data });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyOffers()
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }
            var data = await _offerService.GetMyOffersAsync(userId);
            return Ok(new ApiResponse<IEnumerable<GetOfferDTO>> { Data = data });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _offerService.GetByIdAsync(id);
            return Ok(new ApiResponse<GetOfferDTO> { Data = data });
        }

        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetOfferDetail(Guid id)
        {
            var data = await _offerService.GetOfferDetailAsync(id);
            return Ok(new ApiResponse<OfferDetailResponseDto> { Data = data });
        }

        [HttpPost("shared-ride")]
        [Authorize]
        public async Task<IActionResult> CreateSharedRideOffer([FromBody] CreateOfferSharedRidelRequestDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }
            var offer = await _offerService.CreateWithDetailAsync(userId, request);
            return Ok(new ApiResponse<GetOfferDTO> { Data = offer });
        }

        [HttpPost("shipment")]
        [Authorize]
        public async Task<IActionResult> CreateShipmentOffer([FromBody] CreateOfferShipmentRequestDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }
            var offer = await _offerShipmentSerivce.CreateWithCargoAsync(userId, request);
            return Ok(new ApiResponse<GetOfferDTO> { Data = offer });
        }

        [HttpPatch("{offerId}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid offerId, [FromBody] UpdateOfferRequestDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var offer = await _offerService.UpdateAsync(userId, request);
            return Ok(new ApiResponse<GetOfferDTO> { Data = offer });
        }

        [HttpPost("{offerId}/cancel")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> CancelOffer(Guid offerId)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var offer = await _offerService.CancelOfferAsync(userId, offerId);
            return Ok(new ApiResponse<GetOfferDTO> { Data = offer });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            await _offerService.DeleteAsync(userId, id);
            return Ok(new ApiResponse<string> { Message = "Xóa thành công" });
        }
    }
}
