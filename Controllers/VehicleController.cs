using AppGroupCar.DTO.RequestDTO.Market.Vehicle;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Market;
using AppGroupCar.Services.Market.VehicleServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        /// <summary>
        /// Get driver ID from JWT token
        /// </summary>
        private Guid GetDriverIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                throw new InvalidOperationException("Invalid or missing user ID in token");
            return userId;
        }

        /// <summary>
        /// Create a new vehicle
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateVehicleDTO dto)
        {
            var driverId = GetDriverIdFromToken();
            var result = await _vehicleService.CreateAsync(driverId, dto);
            return Ok(new ApiResponse<VehicleDTO> { Data = result, Message = "Vehicle created successfully" });
        }

        /// <summary>
        /// Get all vehicles of current driver
        /// </summary>
        [HttpGet("my-vehicles")]
        public async Task<IActionResult> GetMyVehicles()
        {
            var driverId = GetDriverIdFromToken();
            var result = await _vehicleService.GetByDriverIdAsync(driverId);
            return Ok(new ApiResponse<IEnumerable<VehicleDTO>> { Data = result, Message = "Vehicles retrieved successfully" });
        }

        /// <summary>
        /// Get vehicle details by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _vehicleService.GetByIdAsync(id);
            return Ok(new ApiResponse<VehicleDTO> { Data = result, Message = "Vehicle retrieved successfully" });
        }

        /// <summary>
        /// Update vehicle information
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateVehicleDTO dto)
        {
            var driverId = GetDriverIdFromToken();
            await _vehicleService.UpdateAsync(id, driverId, dto);
            return Ok(new ApiResponse<object> { Data = null, Message = "Vehicle updated successfully" });
        }

        /// <summary>
        /// Soft delete vehicle
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var driverId = GetDriverIdFromToken();
            await _vehicleService.DeleteAsync(id, driverId);
            return Ok(new ApiResponse<object> { Data = null, Message = "Vehicle deleted successfully" });
        }
    }
}
