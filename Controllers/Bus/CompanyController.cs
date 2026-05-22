using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters.Bus;
using AppGroupCar.DTO.RequestDTO.Bus.CompanyRequest;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyResponse;
using AppGroupCar.Entities.Bus;
using AppGroupCar.Exceptions;
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
        [AllowAnonymous]
        public async Task<IActionResult> CreateCompany([FromForm] CreateCompanyRequestDto dto)
        {
            var result = await _companyService.CreateCompanyAsync(dto);
            return Ok(new ApiResponse<object> { Code = 201, Message = "Create success", Data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _companyService.GetByIdAsync(id);

            if (result == null)
                throw new AppException(404, "Company not found");

            return Ok(new ApiResponse<object> { Data = result });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters param)
        {
            var data = await _companyService.FilterAsync(param);
            return Ok(new ApiResponse<PagedResult<CompanyResponseDto>> { Data = data });
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCompany(
            Guid id,
            [FromForm] UpdateCompanyRequestDto dto)
        {
            var result = await _companyService.UpdateCompanyAsync(id, dto);
            return Ok(new ApiResponse<object> { Data = result });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> CompanyLogin([FromBody] CompanyLoginRequestDto dto)
        {
            var result = await _companyService.LoginCompanyAsync(dto);

            if (result.Status == CompanyStatus.Approved)
            {
                return Ok(new ApiResponse<object>
                {
                    Message = "đã được duyệt vui lòng vào bằng bản web",
                    Data = null
                });
            }

            if (result.Status == CompanyStatus.Pending || result.Status == CompanyStatus.Rejected)
            {
                var message = result.Status == CompanyStatus.Pending
                    ? "Hồ sơ công ty đang ở trạng thái chờ duyệt."
                    : $"Hồ sơ công ty bị từ chối. Lý do: {(string.IsNullOrWhiteSpace(result.Description) ? "Chưa có mô tả lý do." : result.Description)}";

                return Ok(new ApiResponse<object>
                {
                    Message = message,
                    Data = result
                });
            }

            if (result.Status == CompanyStatus.Suspended)
            {
                return Ok(new ApiResponse<object>
                {
                    Message = "Công ty đang bị tạm ngưng, vui lòng liên hệ quản trị viên để được hỗ trợ.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = $"Công ty hiện đang ở trạng thái {result.Status}.",
                Data = null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _companyService.DeleteCompanyAsync(id);

            return Ok(new ApiResponse<object>
            {
                Message = "Company deleted successfully"
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

            return Ok(new ApiResponse<object> { Data = result });
        }

        [HttpPut("{id}/lock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LockCompany(Guid id)
        {
            await _companyService.LockCompanyAsync(id);
            return Ok(new ApiResponse<object> { Data = null, Message = "Khóa thành công" });
        }

        [HttpPut("{id}/unlock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnlockCompany(Guid id)
        {
            await _companyService.UnlockCompanyAsync(id);
            return Ok(new ApiResponse<object> { Data = null, Message = "Mở khóa thành công" });
        }

    }
}


