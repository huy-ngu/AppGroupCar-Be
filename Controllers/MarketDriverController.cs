using AppGroupCar.DTO;
using AppGroupCar.DTO.Filters;
using AppGroupCar.DTO.RequestDTO.Market.MarketDriver;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Entities.Market;
using AppGroupCar.Helpers;
using AppGroupCar.Services.Market.MarketDriverServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppGroupCar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketDriverController : ControllerBase
    {
        private readonly IMarketDriverService _marketDriverService;
        public MarketDriverController(IMarketDriverService marketDriverService)
        {
            _marketDriverService = marketDriverService;
        }
        [HttpGet("id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _marketDriverService.GetByIdAsync(id);
            return Ok(new ApiResponse<Driver> { Data = data });
        }
        [HttpPost("filter")]
        public async Task<IActionResult> Filter(MarketDriverParameters param)
        {
            var data = await _marketDriverService.FilterAsync(param);
            return Ok(new ApiResponse<PagedResult<Driver>> { Data = data });
        }



        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateDriverRequestDTO request)
        {
            var userIdNullable = User.GetUserId();
            if (userIdNullable is not Guid userId)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }
            var fullName = User.FindFirstValue("fullname");
            if (fullName is null)
            {
                return Unauthorized(new ApiResponse<string> { Message = "Token không hợp lệ" });
            }

            var driver = await _marketDriverService.CreatAsync(userId, fullName, request);
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
    }
}
