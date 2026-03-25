using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters.Bus;
using AppGroupCar.DTO.RequestDTO.Bus.CompanyRequest;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyResponse;
using AppGroupCar.Services.Bus.CompanyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppGroupCar.Controllers.Bus
{
    [ApiController]
    [Route("companies")]
    [Authorize(Roles = "Admin")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromForm] CreateCompanyRequestDto dto)
        {

            var result = await _companyService.CreateCompanyAsync(dto);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _companyService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Company not found" });

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCompanies([FromQuery]CompanyParameters param)
        {
            var data = await _companyService.FilterAsync(param);
            return Ok(new ApiResponse<PagedResult<CompanyResponseDto>> { Data = data });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(
            Guid id,
            [FromForm] UpdateCompanyRequestDto dto)
        {
            var result = await _companyService.UpdateCompanyAsync(id, dto);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _companyService.DeleteCompanyAsync(id);

            return Ok(new
            {
                message = "Company deleted successfully"
            });
        }

        //api lấy danh sách công ty đang ở trạng thái pending
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingCompanies([FromQuery] CompanyParameters param)
        {
            var data = await _companyService.GetPendingCompaniesAsync(param);
            return Ok(new ApiResponse<PagedResult<CompanyResponseDto>> { Data = data });
        }

        //api chuyển trạng thái công ty từ pending sang approved
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveCompany(
            Guid id,
            [FromBody] ApproveCompanyRequestDto dto)
        {
            var result = await _companyService.ApproveCompanyAsync(id, dto);

            return Ok(result);
        }

    }
}
