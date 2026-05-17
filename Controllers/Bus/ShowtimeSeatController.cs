using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Services.Bus.ShowtimeSeatServices;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Bus
{
    [ApiController]
    [Route("showtimes")]
    public class ShowtimeSeatController : ControllerBase
    {
        private readonly IShowtimeSeatService _service;

        public ShowtimeSeatController(IShowtimeSeatService service)
        {
            _service = service;
        }

        [HttpGet("{id:guid}/seats")]
        public async Task<IActionResult> GetSeatMap(Guid id)
        {
            var result = await _service.GetSeatMapAsync(id);
            return Ok(new ApiResponse<object> { Data = result });
        }
    }
}
