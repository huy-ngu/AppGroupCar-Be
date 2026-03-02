using AppGroupCar.DTO.RequestDTO;
using AppGroupCar.Services.RideRequestServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RideRequestController : ControllerBase
    {
        private readonly IRideRequestService _service;

        public RideRequestController(IRideRequestService service)
        {
            _service = service;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateRideRequest(CreateRideRequestDTO rideRequest)
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Id == null)
            {
                return Unauthorized("Sai token");
            }
            Guid userId = new Guid(Id);
            var newRideRequest = await _service.CreateRideRequestAsync(userId, rideRequest);
            return Ok(newRideRequest);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllRecords()
        {
            var data = await _service.GetAllRideRequestAsync();
            return Ok(data);
        }
        [HttpPost("FilterByNameAsync")]
        public async Task<IActionResult> FilterByName(string name, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _service.FindByNameAsync(name, pageNumber, pageSize);
            return Ok(data);
        }
        [HttpPost("FilterByTimeAndLocation")]
        public async Task<IActionResult> FilterByTimeAndLocation(string? fromlocation, string? toLocation, DateTime? fromTime, DateTime? toTime)
        {
            var data = await _service.FilterByTimeAndLocationAsync(fromlocation, toLocation, fromTime, toTime);
            return Ok(data);
        }
    }
}
