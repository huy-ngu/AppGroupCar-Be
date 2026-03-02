using AppGroupCar.Entities;
using AppGroupCar.Services.LocationServices;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            return Ok(location);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllLocation()
        {
            var location = await _locationService.GetAllAsync();
            return Ok(location);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateLocation(string name, LocationType type)
        {
            var location = await _locationService.CreateLocationAsync(name, type);
            return Ok(location);
        }
    }
}
