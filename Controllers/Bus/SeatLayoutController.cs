using AppGroupCar.DTO;
using AppGroupCar.DTO.RequestDTO.Bus.SeatLayoutRequest;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Bus.SeatLayoutResponse;
using AppGroupCar.Exceptions;
using AppGroupCar.Services.Bus.SeatLayoutServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Bus
{
    [ApiController]
    [Route("seat-layouts")]
    //[Authorize(Roles = "ADMIN_COMPANY")]
    public class SeatLayoutController : ControllerBase
    {
        private readonly ISeatLayoutService _service;

        public SeatLayoutController(ISeatLayoutService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVehicleSeatLayoutRequest dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(new ApiResponse<object> { Code = 201, Message = "Create success", Data = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new ApiResponse<IEnumerable<VehicleSeatLayoutResponseDto>> { Data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(new ApiResponse<VehicleSeatLayoutResponseDto> { Data = result });
        }
    }
}
