using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Bus.ShowtimeRequest;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Bus.RouteResponse;
using AppGroupCar.DTO.ResponseDTO.Bus.ShowtimeResponse;
using AppGroupCar.Exceptions;
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
                throw new AppException(401, "Company not found in token");

            return companyId.Value;
        }

        private Guid GetUserId()
        {
            var userId = User.GetUserId();

            if (userId == null)
                throw new AppException(401, "User not found in token");

            return userId.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBusShowtimeRequest dto)
        {
            var companyId = GetCompanyId();
            var userId = GetUserId();

            var result = await _service.CreateAsync(companyId, userId, dto);
            return Ok(new ApiResponse<BusShowtimeResponse> { Data = result });
        }

        [HttpGet]
        public async Task<IActionResult> Filter(
            [FromQuery] BusShowtimeParameters param)
        {
            var companyId = GetCompanyId();
            var result = await _service.FilterAsync(companyId, param);

            return Ok(new ApiResponse<PagedResult<BusShowtimeResponse>> { Data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetById(id);
            return Ok(new ApiResponse<BusShowtimeResponse> { Data = result });
        }
        
        [AllowAnonymous]
        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetPublicDetail(Guid id)
        {
            var result = await _service.GetPublicDetailAsync(id);
            return Ok(new ApiResponse<BusShowtimeDetailResponseDto> { Data = result });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBusShowtimeRequest dto)
        {
            var companyId = GetCompanyId();
            var result = await _service.UpdateAsync(id, companyId, dto);

            return Ok(new ApiResponse<BusShowtimeResponse> { Data = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var companyId = GetCompanyId();
            var userId = GetUserId();

            await _service.DeleteAsync(id, companyId, userId);
            return Ok(new ApiResponse<string> { });
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeBusShowtimeStatusRequest dto)
        {
            var companyId = GetCompanyId();
            var userId = GetUserId();

            var result = await _service.ChangeStatusAsync(id, companyId, userId, dto);
            return Ok(new ApiResponse<BusShowtimeResponse> { Data = result });
        }
    }
}
