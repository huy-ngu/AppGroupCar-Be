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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppGroupCar.Controllers.Market
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService _offerService;
        private readonly IOfferShipmentSerivce _offerShipmentSerivce;

        public OfferController(IOfferService offerService, IOfferShipmentSerivce offerShipmentSerivce)
        {
            _offerService = offerService;
            _offerShipmentSerivce = offerShipmentSerivce;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _offerService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<GetOfferDTO>> { Data = data });
        }

        [HttpGet("my-offers")]
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

            var offer = await _offerService.UpdateAsync(userId, offerId, request);
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
