using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Search;
using AppGroupCar.Services.Search;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly ITripSearchService _tripSearchService;

        public SearchController(ITripSearchService tripSearchService)
        {
            _tripSearchService = tripSearchService;
        }

        [HttpGet("trips")]
        public async Task<IActionResult> SearchTrips([FromQuery] TripSearchParameters parameters)
        {
            var result = await _tripSearchService.SearchTripsAsync(parameters);
            return Ok(new ApiResponse<PagedResult<TripSearchItemDTO>> { Data = result });
        }
    }
}
