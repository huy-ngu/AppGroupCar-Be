using AppGroupCar.DTO.RequestDTO.Bus.SeatLayoutRequest;
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
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
    }
}
