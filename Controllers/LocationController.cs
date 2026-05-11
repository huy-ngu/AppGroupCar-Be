using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities;
using AppGroupCar.Services.Mytoys.LocationServices;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _locationService;

        public LocationController(LocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetLocationById(Guid id)
        {
            var location = await _locationService.GetLocationByIdAsync(id);
            return Ok(new ApiResponse<Location?> { Data = location });
        }
        [HttpGet("")]
        public async Task<IActionResult> SearchLocations([FromQuery] LocationSearchParameters parameters)
        {
            var locations = await _locationService.SearchLocationsAsync(parameters);
            return Ok(new ApiResponse<PagedResult<LocationSearchResultDTO>> { Data = locations });
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllLocation()
        {
            var location = await _locationService.GetAllAsync();
            return Ok(new ApiResponse<object> { Data = location });
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateLocation(CreateLocationRequestDTO request)
        {
            var location = await _locationService.CreateLocationAsync(request);
            return Ok(new ApiResponse<Location> { Data = location });
        }
    }
}
