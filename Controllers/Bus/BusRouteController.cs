using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Bus.RouteRequest;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.DTO.ResponseDTO.Bus.CompanyResponse;
using AppGroupCar.DTO.ResponseDTO.Bus.RouteResponse;
using AppGroupCar.Exceptions;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Bus.BusRoute;
using Hangfire.MemoryStorage.Database;
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
                throw new AppException(401, "Company not found in token");

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

            return Ok(new ApiResponse<BusRouteResponse> { Data = result });
        }

        // GET ALL

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] BusRouteParameters param)
        {
            var companyId = GetCompanyId();

            var result = await _service.GetAsync(companyId, param);

            return Ok(new ApiResponse<PagedResult<BusRouteResponse>> { Data = result });
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
                throw new AppException(404, "Bus route not found");

            return Ok(new ApiResponse<BusRouteResponse> { Data = result });
        }

        // UPDATE

        [HttpPut("{id}")]
        public async Task<IActionResult>
            Update(Guid id, UpdateBusRouteRequest dto)
        {
            var companyId = GetCompanyId();

            var result =
                await _service.UpdateAsync(id, companyId, dto);

            return Ok(new ApiResponse<BusRouteResponse> { Data = result });
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
