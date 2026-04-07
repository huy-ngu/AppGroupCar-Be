using AppGroupCar.DTO.RequestDTO.Bus.ShowtimeRequest;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Bus.ShowtimeServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Bus
{
    [ApiController]
    [Route("bus-showtimes")]
    [Authorize(Roles = "ADMIN_COMPANY")]
    public class BusShowtimeController : ControllerBase
    {
        private readonly IBusShowtimeService _service;

        public BusShowtimeController(IBusShowtimeService service)
        {
            _service = service;
        }

        private Guid GetCompanyId()
        {
            var companyId = User.GetCompanyId();

            if (companyId == null)
                throw new Exception("Company not found in token");

            return companyId.Value;
        }

        private Guid GetUserId()
        {
            var userId = User.GetUserId();

            if (userId == null)
                throw new Exception("User not found in token");

            return userId.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBusShowtimeRequest dto)
        {
            var companyId = GetCompanyId();
            var userId = GetUserId();

            var result = await _service.CreateAsync(companyId, userId, dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companyId = GetCompanyId();
            var result = await _service.GetByCompanyAsync(companyId);

            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
            [FromQuery] DateOnly departureDate,
            [FromQuery] string? plateNumber,
            [FromQuery] TimeOnly? departureTime)
        {
            var companyId = GetCompanyId();
            var result = await _service.FilterAsync(companyId, departureDate, plateNumber, departureTime);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBusShowtimeRequest dto)
        {
            var companyId = GetCompanyId();
            var result = await _service.UpdateAsync(id, companyId, dto);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var companyId = GetCompanyId();
            var userId = GetUserId();

            await _service.DeleteAsync(id, companyId, userId);
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeBusShowtimeStatusRequest dto)
        {
            var companyId = GetCompanyId();
            var userId = GetUserId();

            var result = await _service.ChangeStatusAsync(id, companyId, userId, dto);
            return Ok(result);
        }
    }
}
