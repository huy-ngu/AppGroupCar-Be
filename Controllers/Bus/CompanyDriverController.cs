using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Bus.CompanyDriverRequest;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Exceptions;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Bus.CompanyDriverServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Bus
{
    [ApiController]
    [Route("company-drivers")]
    [Authorize(Roles = "ADMIN_COMPANY")]
    public class CompanyDriverController : ControllerBase
    {
        private readonly ICompanyDriverService _service;

        public CompanyDriverController(ICompanyDriverService service)
        {
            _service = service;
        }

        private Guid GetCompanyId()
        {
            var companyId = User.GetCompanyId();

            if (companyId == null)
                throw new AppException(401, "Company not found in token");

            return companyId.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCompanyDriverRequestDto dto)
        {
            var companyId = GetCompanyId();
            var result = await _service.CreateAsync(companyId, dto);

            return Ok(new ApiResponse<object> { Code = 201, Message = "Create success", Data = result });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] CompanyDriverParameters param)
        {
            var companyId = GetCompanyId();
            var result = await _service.GetByCompanyAsync(param, companyId);

            return Ok(new ApiResponse<object> { Data = result });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, Guid companyId)
        {
            var result = await _service.GetByIdAsync(id, companyId);

            if (result == null)
                throw new AppException(404, "Company driver not found");

            return Ok(new ApiResponse<object> { Data = result });
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateCompanyDriverRequestDto dto)
        {
            var companyId = GetCompanyId();
            var result = await _service.UpdateAsync(id, companyId, dto);

            return Ok(new ApiResponse<object> { Data = result });
        }
        [HttpPut("{id}/status")]
        [AllowAnonymous]

        public async Task<IActionResult> UpdateStatus(Guid id,Guid companyId, [FromForm] UpdateCompanyDriverRequestDto dto)
        {
            var result = await _service.UpdateAsync(id, companyId, dto);

            return Ok(new ApiResponse<object> { Data = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var companyId = GetCompanyId();

            await _service.DeleteAsync(id, companyId);

            return Ok(new ApiResponse<object> { Message = "Company driver deleted (soft)" });
        }

        [HttpGet("company/{companyId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCompanyForAdmin(
            [FromRoute] Guid companyId,
            [FromQuery] CompanyDriverParameters param)
        {
            var result = await _service.GetByCompanyAsync(param, companyId);
            return Ok(new ApiResponse<object> { Data = result });
        }
    }
}
