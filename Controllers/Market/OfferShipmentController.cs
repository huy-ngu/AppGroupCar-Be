using AppGroupCar.DTO.RequestDTO.Market.Offer;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market.Offer;
using AppGroupCar.Entities.Market;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Market.OfferServices.OfferShipmentServices;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Market
{
    [ApiController]
    [Route("[controller]")]
    public class OfferShipmentController : ControllerBase
    {
        private readonly IOfferShipmentSerivce _offerShipmentSerivce;

        public OfferShipmentController(IOfferShipmentSerivce offerShipmentSerivce)
        {
            _offerShipmentSerivce = offerShipmentSerivce;
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateOfferShipment(CreateOfferShipmentRequestDTO request)
        //{
        //    var userIdNullable = User.GetUserId();
        //    if (userIdNullable is not Guid userId)
        //    {
        //        return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
        //    }
        //    var data = await _offerShipmentSerivce.CreateWithCargoAsync(userId, request);
        //    return Ok(new ApiResponse<OfferCargoDTO> { Data = data });

        //}
    }
}
