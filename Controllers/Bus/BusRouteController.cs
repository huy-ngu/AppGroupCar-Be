using AppGroupCar.DTO.RequestDTO.Bus.RouteRequest;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Bus.BusRoute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Bus
{
    [ApiController]
    [Route("bus-routes")]
    [Authorize(Roles = "ADMIN_COMPANY")]
    public class BusRouteController : ControllerBase
    {
        private readonly IBusRouteService _service;

        public BusRouteController(
            IBusRouteService service)
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

        // CREATE

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateBusRouteRequest dto)
        {
            var companyId = GetCompanyId();

            var result =
                await _service.CreateAsync(companyId, dto);

            return Ok(result);
        }

        // GET ALL

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companyId = GetCompanyId();

            var result =
                await _service
                    .GetByCompanyAsync(companyId);

            return Ok(result);
        }

        // GET BY ID

        [HttpGet("{id}")]
        public async Task<IActionResult>
            GetById(Guid id)
        {
            var companyId = GetCompanyId();

            var result =
                await _service
                    .GetByIdAsync(id, companyId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // UPDATE

        [HttpPut("{id}")]
        public async Task<IActionResult>
            Update(Guid id, UpdateBusRouteRequest dto)
        {
            var companyId = GetCompanyId();

            var result =
                await _service.UpdateAsync(id, companyId, dto);

            return Ok(result);
        }

        // DELETE

        [HttpDelete("{id}")]
        public async Task<IActionResult>
            Delete(Guid id)
        {
            var companyId = GetCompanyId();

            await _service.DeleteAsync(id, companyId);

            return NoContent();
        }
    }
}
