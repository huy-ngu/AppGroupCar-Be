using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Bus.VehicleRequest;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Bus.CompanyVehicleServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Bus
{
    [ApiController]
    [Route("company-vehicles")]
    [Authorize(Roles = "ADMIN_COMPANY")]
    public class CompanyVehicleController : ControllerBase
    {
        private readonly ICompanyVehicleService _service;

        public CompanyVehicleController(ICompanyVehicleService service)
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

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCompanyVehicleRequest dto)
        {
            var companyId = GetCompanyId();
            var result = await _service.CreateAsync(companyId, dto);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var companyId = GetCompanyId();
            var result = await _service.GetByIdAsync(id, companyId);

            return result == null ? NotFound(new { message = "Company vehicle not found" }) : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CompanyVehicleParameters param)
        {
            var companyId = GetCompanyId();
            var result = await _service.GetByCompanyAsync(param, companyId);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateCompanyVehicleRequest dto)
        {
            var companyId = GetCompanyId();
            var result = await _service.UpdateAsync(id, companyId, dto);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var companyId = GetCompanyId();

            await _service.DeleteAsync(id, companyId);

            return Ok(new { message = "Company vehicle deleted (soft)" });
        }
    }
}
