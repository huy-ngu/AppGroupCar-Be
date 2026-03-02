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
        [HttpPost]
        public async Task<IActionResult> CreateRideRequest(CreateRideRequestDTO rideRequest)
        {
            var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Id == null)
            {
                return Unauthorized("Sai token");
            }
            Guid userId = new Guid(Id);
            var newRideRequest = await _service.createRideRequestAsync(userId, rideRequest);
            return Ok(newRideRequest);
        }
    }
}
