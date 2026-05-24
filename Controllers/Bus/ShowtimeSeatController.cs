using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Bus.SeatResponse;
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
            return Ok(new ApiResponse<List<ShowtimeSeatResponseDto>> { Data = result });
        }

        [HttpGet("{id:guid}/seat-layout")]
        public async Task<IActionResult> GetSeatLayout(Guid id)
        {
            var result = await _service.GetSeatLayoutAsync(id);
            return Ok(new ApiResponse<SeatLayoutSnapshotDto> { Data = result });
        }

        [HttpGet("{id:guid}/seat-map")]
        public async Task<IActionResult> GetSeatMapWithLayout(Guid id)
        {
            var result = await _service.GetSeatMapWithLayoutAsync(id);
            return Ok(new ApiResponse<ShowtimeSeatMapResponseDto> { Data = result });
        }
    }
}
