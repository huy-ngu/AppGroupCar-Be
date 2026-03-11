using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Market.OfferServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OfferController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _offerService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<Offer>> { Data = data });
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
            return Ok(new ApiResponse<IEnumerable<Offer>> { Data = data });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _offerService.GetByIdAsync(id);
            return Ok(new ApiResponse<Offer> { Data = data });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateOfferRequestDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }
            var offer = await _offerService.CreateAsync(userId, request);
            return Ok(new ApiResponse<Offer> { Data = offer });
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
            return Ok(new ApiResponse<Offer> { Data = offer });
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
