using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Services.Bus.UserCompanyServices;
using AppGroupCar.Services.Market.OfferRoutePointServices;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Market
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfferRoutePointController : ControllerBase
    {
        private readonly IOfferRoutePointService _offerRoutePointService;
        public OfferRoutePointController(IOfferRoutePointService offerRoutePointService)
        {
            _offerRoutePointService = offerRoutePointService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRecords()
        {
            var data = await _offerRoutePointService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<OfferRoutePoint>> { Data = data });
        }
    }
}
