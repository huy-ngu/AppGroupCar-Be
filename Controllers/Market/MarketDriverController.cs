using AppGroupCar.DTO;
using AppGroupCar.DTO.PagingParam;
using AppGroupCar.DTO.RequestDTO.Market.MarketDriver;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Market.MarketDriverServices;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppGroupCar.Controllers.Market
{
    [ApiController]
    [Route("[controller]")]
    public class MarketDriverController : ControllerBase
    {
        private readonly IMarketDriverService _marketDriverService;
        public MarketDriverController(IMarketDriverService marketDriverService)
        {
            _marketDriverService = marketDriverService;
        }
        [HttpGet]
        public async Task<IActionResult> GetDrivers([FromQuery] DriverParameters parameters)
        {
            var data = await _marketDriverService.GetMarketDriversAsync(parameters);
            return Ok(new ApiResponse<PagedResult<Driver>> { Data = data });

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _marketDriverService.GetByIdAsync(id);
            return Ok(new ApiResponse<Driver> { Data = data });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateDriverRequestDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }
            var driver = await _marketDriverService.CreatAsync(userId, request);
            return Ok(new ApiResponse<Driver> { Data = driver });
        }
        [HttpPatch("update")]
        public async Task<IActionResult> Update([FromForm] UpdateDriverRequestDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var driver = await _marketDriverService.UpdateAsync(userId, request);
            return Ok(new ApiResponse<Driver> { Data = driver });

        }

        [HttpPatch("approval")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApprovalDriver(Guid userId)
        {
            await _marketDriverService.ApprovalDriverAsync(userId);
            return Ok(new ApiResponse<Driver> { });
        }

        [HttpPatch("refuse")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RefuseDriver(Guid userId)
        {
            await _marketDriverService.RefuseDriverAsync(userId);
            return Ok(new ApiResponse<Driver> { });
        }
    }
}
