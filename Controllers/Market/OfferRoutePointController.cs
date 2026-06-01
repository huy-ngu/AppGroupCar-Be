using AppGroupCar.DTO.RequestDTO.Market.OfferRoutePoint;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Services.Market.OfferRoutePointServices;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Market
{
    [ApiController]
    [Route("[controller]")]
    public class OfferRoutePointController : ControllerBase
    {
        private readonly IOfferRoutePointService _offerRoutePointService;

        public OfferRoutePointController(IOfferRoutePointService offerRoutePointService)
        {
            _offerRoutePointService = offerRoutePointService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _offerRoutePointService.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<OfferRoutePoint>> { Data = data });
        }
        [HttpGet("OfferId")]
        public async Task<IActionResult> GetByOffer([FromQuery] Guid OfferId)
        {
            var data = await _offerRoutePointService.GetByOfferIdOrderedAsync(OfferId);
            return Ok(new ApiResponse<IEnumerable<OfferRoutePoint>> { Data = data });
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateOfferRoutePointRequestDTO request)
        {
            var data = await _offerRoutePointService.CreateAsync(request);
            return Ok(new ApiResponse<OfferRoutePoint> { Data = data });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateOfferRoutePointRequestDTO request)
        {
            await _offerRoutePointService.UpdateAsync(id, request);
            return Ok(new ApiResponse<object?> { Data = null });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var data = await _offerRoutePointService.DeleteAsync(id);
            return Ok(new ApiResponse<bool> { Data = data });
        }
    }
}
